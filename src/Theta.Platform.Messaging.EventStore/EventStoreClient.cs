using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
        private readonly IEventStoreConnection _connection;
        private bool _connected;

        public EventStoreClient(IEventStoreConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.Create();
        }

        public async Task<IEvent[]> Retrieve(Type eventType)
        {
            // TODO: System projections must be enabled on the eventstore for this to work.
            var streamName = $"$et-{eventType.Name}";

            var sliceStart = 0L;
            var deserializedEvents = new List<IEvent>();
            StreamEventsSlice slice;

            do
            {
                slice = await _connection.ReadStreamEventsForwardAsync(streamName, sliceStart, 200, true);

                var events = slice.Events;

                foreach (var evt in events)
                {
                    var item = Deserialize<IEvent>(evt.Event);

                    if (item != null)
                    {
                        deserializedEvents.Add(item);
                    }
                }

                //deserializedEvents.AddRange(events.Select(x => ));
                sliceStart = slice.NextEventNumber;
            } while (!slice.IsEndOfStream);

            return deserializedEvents.ToArray();
        }

        public async Task<IEvent[]> RetrieveAll()
        {
            await EnsureConnected();

            var sliceStart = Position.Start;
            var deserializedEvents = new List<IEvent>();
            AllEventsSlice slice;

            do
            {
                slice = await _connection.ReadAllEventsForwardAsync(sliceStart, 200, true);
                deserializedEvents.AddRange(slice.Events.Select(x => Deserialize<IEvent>(x.Event)));
                sliceStart = slice.NextPosition;
            } while (!slice.IsEndOfStream);

            return deserializedEvents.ToArray();
        }

        public async Task<IEvent[]> Retrieve(string streamName)
        {
            await EnsureConnected();

            var sliceStart = 0L;
            var deserializedEvents = new List<IEvent>();
            StreamEventsSlice slice;

            do
            {
                slice = await _connection.ReadStreamEventsForwardAsync(streamName, sliceStart, 200, true);
                deserializedEvents.AddRange(slice.Events.Select(x => Deserialize<IEvent>(x.Event)));
                sliceStart = slice.NextEventNumber;
            } while (!slice.IsEndOfStream);

            return deserializedEvents.ToArray();
        }

        public async Task Save(string streamName, int expectedVersion, IEvent domainEvent)
        {
            await EnsureConnected();

            var serializedEvent = Serialize(domainEvent);
            var eventData = new EventData(Guid.NewGuid(), domainEvent.GetType().Name, true, serializedEvent, null);

            await _connection.AppendToStreamAsync(streamName, expectedVersion, eventData);
        }

        public async Task<IObservable<IEvent>> SubscribeToAll()
        {
            await EnsureConnected();

            return Observable.Create(
                async (IObserver<IEvent> obs) =>
                {
                    var onEventReceived = new Func<EventStoreSubscription, ResolvedEvent, Task>(
                        (subscription, ev) =>
                        {
                            var domainEvent = Deserialize<OrderCreatedEvent>(ev.Event);

                            if (domainEvent == null)
                                return Task.CompletedTask;

                            obs.OnNext(domainEvent);
                            return Task.CompletedTask;
                        });

                    await _connection.SubscribeToAllAsync(true, onEventReceived);
                });
        }

        public async Task<IObservable<TEvent>> Subscribe<TEvent>() where TEvent : IEvent
        {
            await EnsureConnected();

            // TODO: System projections must be enabled on the eventstore for this to work.
            var streamName = $"$et-{typeof(TEvent).Name}";

            return Observable.Create(
                async (IObserver<TEvent> obs) =>
                {
                    var onEventReceived = new Func<EventStoreSubscription, ResolvedEvent, Task>(
                        (subscription, ev) =>
                        {
                            var domainEvent = Deserialize<TEvent>(ev.Event);
                            obs.OnNext(domainEvent);
                            return Task.CompletedTask;
                        });

                    await _connection.SubscribeToStreamAsync(streamName, true, onEventReceived);
                });
        }

        public async Task<IObservable<IEvent>> Subscribe(string streamName)
        {
            await EnsureConnected();

            return Observable.Create(
                async (IObserver<IEvent> obs) =>
                {
                    var onEventReceived = new Func<EventStoreSubscription, ResolvedEvent, Task>(
                        (subscription, ev) =>
                        {
                            var domainEvent = Deserialize<IEvent>(ev.Event);
                            obs.OnNext(domainEvent);
                            return Task.CompletedTask;
                        });

                    await _connection.SubscribeToStreamAsync(streamName, true, onEventReceived);
                });
        }

        public async Task Publish(IEvent domainEvent)
        {
            // Not needed for EventStore - the publish happens automatically after Save()
            await Task.CompletedTask;
        }

        private async Task EnsureConnected()
        {
            if (!_connected)
            {
                await _connection.ConnectAsync();
                _connected = true;
            }
        }

        private TEvent Deserialize<TEvent>(RecordedEvent recordedEvent) where TEvent : IEvent
        {

            if (recordedEvent == null)
                return default(TEvent);

            if (!recordedEvent.IsJson)
                return default(TEvent);

            if (recordedEvent.EventType != "OrderCreatedEvent")
                return default(TEvent);

            var data = Encoding.UTF8.GetString(recordedEvent.Data);

            try
            {
                return JsonConvert
                    .DeserializeObject<TEvent>(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            }
            catch (Exception ex)
            {
                // TODO Handle
                return default(TEvent);
            }
        }

        private byte[] Serialize(IEvent evt)
        {
            return Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(evt, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects }));
        }
    }
}