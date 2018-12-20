using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain
{
	public abstract class AggregateReader<TAggregate> : IAggregateReader<TAggregate> where TAggregate : class, IAggregateRoot
	{
		private readonly SerialDisposable _eventSubscription = new SerialDisposable();
		private readonly ConcurrentStack<IEvent> _bufferedEvents = new ConcurrentStack<IEvent>();

		protected readonly IEventPersistenceClient _eventPersistenceClient;
		protected readonly IEventStreamingClient _eventStreamingClient;
		protected readonly ConcurrentDictionary<Guid, TAggregate> aggregateCache = new ConcurrentDictionary<Guid, TAggregate>();

		private bool _retrievedStateOfTheWorld;

		protected AggregateReader(
			IEventPersistenceClient eventPersistenceClient, 
			IEventStreamingClient eventStreamingClient)
		{
			_eventPersistenceClient = eventPersistenceClient;
			_eventStreamingClient = eventStreamingClient;
		}

		protected abstract Dictionary<string, Type> SubscribedEventTypes { get; }

		public async Task StartAsync()
		{
			await _eventStreamingClient.Connect();

			// Wait for the event streaming client to report itself as fully connected before
			// continuing with retrieving persisted events & initializing the streaming subscription.
			_eventStreamingClient.ConnectionStateChanged
				.Where(state => state == StreamingConnectionState.Connected)
				.Take(1)
				.Subscribe(async _ => await Initialize());
		}

		private async Task Initialize()
		{
			// Initialize the streaming subscription
			_eventSubscription.Disposable = _eventStreamingClient.GetAllEventsStream()
				.Where(ev => SubscribedEventTypes.ContainsKey(ev.Type))
				.Subscribe(async e =>
				{
					_bufferedEvents.Push(e);

					if (_retrievedStateOfTheWorld)
					{
						await ProcessBufferedEvents();
					}
				});

			// Retrieve the state of the world & process its events
			var events = await RetrievePersistedEvents();
			events.ForEach(async e => await ProcessEvent(e));

			// Process any streamed events that were buffered meanwhile
			_retrievedStateOfTheWorld = true;
			await ProcessBufferedEvents();
		}

		private async Task<List<IEvent>> RetrievePersistedEvents()
		{
			var retrievedEvents = await Task.WhenAll(
				SubscribedEventTypes
					.Select(async nameToTypeMapping =>
					{
						var type = nameToTypeMapping.Value;
						return await _eventPersistenceClient.Retrieve(type);
					}));

			return retrievedEvents
				.SelectMany(y => y)
				.ToList();
		}

		public TAggregate GetById(Guid id)
		{
			return !aggregateCache.TryGetValue(id, out TAggregate aggregate) ? null : aggregate;
		}

		public TAggregate[] Get()
		{
			return aggregateCache.Values.ToArray();
		}

		private async Task ProcessBufferedEvents()
		{
			while (!_bufferedEvents.IsEmpty)
			{
				_bufferedEvents.TryPop(out IEvent ev);
				await ProcessEvent(ev);
			}
		}

		private Task ProcessEvent(IEvent evt)
		{
			if (!aggregateCache.TryGetValue(evt.AggregateId, out TAggregate aggregate))
			{
				var aggregateRoot = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
				aggregateRoot.Apply(evt);

				if (!aggregateCache.TryAdd(evt.AggregateId, aggregateRoot))
				{
					// TODO: Logging here
					// TODO: Should we retry in this case? Out-of-order event considerations?
					throw new Exception($"Failed to add a new aggregate to in memory cache [Id={evt.AggregateId}, EventType={evt.Type}]");
				}

				return Task.CompletedTask;
			}

			aggregate.Apply(evt);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_eventSubscription.Dispose();
		}
	}
}