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
		protected readonly Dictionary<Guid, TAggregate> aggregateCache = new Dictionary<Guid, TAggregate>();

		private bool _retrievedStateOfTheWorld;

		protected AggregateReader(
			IEventPersistenceClient eventPersistenceClient, 
			IEventStreamingClient eventStreamingClient)
		{
			_eventPersistenceClient = eventPersistenceClient;
			_eventStreamingClient = eventStreamingClient;
		}

		public abstract Dictionary<string, Type> GetEventTypes();

		public async Task StartAsync()
		{
			var allEvents = await _eventStreamingClient.SubscribeToAll();

			_eventSubscription.Disposable = allEvents
				.Where(ev => GetEventTypes().ContainsKey(ev.Type))
				.Subscribe(async e =>
				{
					_bufferedEvents.Push(e);

					if (_retrievedStateOfTheWorld)
					{
						await ProcessBufferedEvents();
					}
				});

			var events = new List<IEvent>();
			foreach (var eventkvp in GetEventTypes())
			{
				var type = eventkvp.Value;
				var retrievedEvents = await _eventPersistenceClient.Retrieve(type);
				events.AddRange(retrievedEvents);
			}

			events.ForEach(async e => await ProcessEvent(e));
			_retrievedStateOfTheWorld = true;
			await ProcessBufferedEvents();
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

				aggregateCache.Add(evt.AggregateId, aggregateRoot);
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