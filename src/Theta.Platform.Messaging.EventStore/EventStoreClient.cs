using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Messaging.EventStore.Factories;

namespace Theta.Platform.Messaging.EventStore
{
	public sealed class EventStoreClient : IEventPersistenceClient, IEventStreamingClient
	{
		private readonly Dictionary<string, Type> _eventTypeDictionary;
		private readonly IEventStoreConnection _connection;
		private readonly Subject<Unit> _connectingSubject = new Subject<Unit>();

		private readonly IDisposable _connectionStateSubscription;
		private readonly SerialDisposable _allEventsSubscription = new SerialDisposable();

		public EventStoreClient(
			Dictionary<string, Type> eventTypeDictionary, 
			IEventStoreConnectionFactory connectionFactory)
		{
			_eventTypeDictionary = eventTypeDictionary;
			_connection = connectionFactory.Create();
			_connectionStateSubscription = CreateConnectionStateChangedStream()
				.Subscribe(cs => this.ConnectionState = cs);
		}

		public async Task Connect()
		{
			_connectingSubject.OnNext(Unit.Default);
			await _connection.ConnectAsync();
		}

		public async Task<IEvent[]> Retrieve(Type eventType)
		{
			EnsureConnected();

			var streamName = $"$et-{eventType.Name}";

			var sliceStart = 0L;
			var deserializedEvents = new List<IEvent>();
			StreamEventsSlice slice;

			do
			{
				slice = await _connection.ReadStreamEventsForwardAsync(streamName, sliceStart, 200, true);
				
				slice.Events
					.ToList()
					.ForEach(evt => ProcessReceivedEvent(evt, deserializedEvents.Add));

				sliceStart = slice.NextEventNumber;
			} while (!slice.IsEndOfStream);

			return deserializedEvents.ToArray();
		}

		private void EnsureConnected()
		{
			if (this.ConnectionState != StreamingConnectionState.Connected)
			{
				throw new InvalidOperationException(
					$"Unable to perform action as underlying EventStoreConnection is not connected. [ConnectionState={ConnectionState.ToString()}");
			}
		}

		public async Task Save(string streamName, int expectedVersion, IEvent domainEvent)
		{
			EnsureConnected();

			var serializedJson = JsonConvert.SerializeObject(domainEvent);
			var encodedEvent = Encoding.UTF8.GetBytes(serializedJson);
			var eventData = new EventData(Guid.NewGuid(), domainEvent.GetType().Name, true, encodedEvent, null);

			await _connection.AppendToStreamAsync(streamName, expectedVersion, eventData);
		}

		public IObservable<IEvent> GetAllEventsStream()
		{
			return Observable.Create(
				async (IObserver<IEvent> obs) =>
				{
					_allEventsSubscription.Disposable = await SubscribeToAllEvents(obs.OnNext, obs.OnError);
					return _allEventsSubscription;
				});
		}

		public async Task Publish(IEvent domainEvent)
		{
			// Not needed for EventStore - the publish happens automatically after Save()
			await Task.CompletedTask;
		}

		public StreamingConnectionState ConnectionState { get; private set; } = StreamingConnectionState.Idle;

		public IObservable<StreamingConnectionState> ConnectionStateChanged => CreateConnectionStateChangedStream();

		private async Task<EventStoreSubscription> SubscribeToAllEvents(Action<IEvent> pumpEvent, Action<Exception> pumpError)
		{
			return await _connection.SubscribeToAllAsync(
				true, 
				(subscription, ev) =>
				{
					try
					{
						ProcessReceivedEvent(ev, pumpEvent);
					}
					catch (Exception ex)
					{
						pumpError(ex);
					}
				}, 
				(subscription, reason, ex) => OnSubscriptionDropped(reason, ex, pumpError, pumpEvent));
		}

		private void OnSubscriptionDropped(SubscriptionDropReason dropReason, Exception ex, Action<Exception> pumpError, Action<IEvent> pumpEvent)
		{
			// If the drop reasons indicate that this was something straightforward, we should recreate the subscription
			if (dropReason == SubscriptionDropReason.UserInitiated ||
			    dropReason == SubscriptionDropReason.EventHandlerException)
			{
				// Wait for the ConnectionState == Connected before recreating
				CreateConnectionStateChangedStream()
					.Where(state => state == StreamingConnectionState.Connected)
					.Take(1)
					.Subscribe(async _ =>
					{
						_allEventsSubscription.Disposable = await SubscribeToAllEvents(pumpEvent, pumpError);
					});

				return;
			}

			// If the connection was dropped for another reason, don't recreate and instead pump an exception to the consumer
			// TODO: Logging here
			pumpError(ex);
		}

		private void ProcessReceivedEvent(ResolvedEvent ev, Action<IEvent> onProcessSuccessful)
		{
			// If we aren't interested in this event type, disregard it
			if (!_eventTypeDictionary.TryGetValue(ev.Event.EventType, out Type eventType))
			{
				// TODO: Logging here
				return;
			}

			try
			{
				var jsonBody = Encoding.UTF8.GetString(ev.Event.Data);
				var typedEvent = (IEvent)JsonConvert.DeserializeObject(jsonBody, eventType);

				typedEvent.EventId = ev.Event.EventId;

				onProcessSuccessful(typedEvent);
			}
			catch (Exception ex)
			{
				// TODO: Logging here
				// Ensure that the exception bubbles up appropriately
				throw new JsonException($"Failed to deserialize event: [Type={ev.Event.EventType}, EventId={ev.Event.EventId}]", ex);
			}
		}

		private IObservable<StreamingConnectionState> CreateConnectionStateChangedStream()
		{
			return Observable.Create(
				(IObserver<StreamingConnectionState> obs) =>
				{
					var connecting = _connectingSubject
						.Subscribe(_ => obs.OnNext(StreamingConnectionState.Connecting));

					var connected = EventStoreHelpers.GetConnectedStream(_connection)
						 .Subscribe(_ => obs.OnNext(StreamingConnectionState.Connected));

					var disconnected = EventStoreHelpers.GetDisconnectedStream(_connection)
						.Subscribe(_ => obs.OnNext(StreamingConnectionState.Disconnected));

					var reconnecting = EventStoreHelpers.GetReconnectingStream(_connection)
						.Subscribe(_ => obs.OnNext(StreamingConnectionState.Reconnecting));

					var closed = EventStoreHelpers.GetClosedStream(_connection)
						.Subscribe(_ => obs.OnNext(StreamingConnectionState.Closed));

					var authFailed = EventStoreHelpers.GetAuthFailedStream(_connection)
						.Subscribe(_ => obs.OnNext(StreamingConnectionState.AuthenticationFailed));

					return new CompositeDisposable(connecting, connected, disconnected, reconnecting, closed, authFailed);
				})
				.StartWith(ConnectionState);
		}

		public void Dispose()
		{
			_connection?.Close();
			_connectionStateSubscription.Dispose();
			_connection?.Dispose();
		}
	}
}