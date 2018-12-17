using EventStore.ClientAPI;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Paltform.Order.Read.Service.Domain.Events;

namespace Theta.Paltform.Order.Read.Service
{
    public class OrdersSubscriber : IOrdersSubscriber
    {
        private readonly IEventStoreConnection _connection;
        private IMemoryCache _cache;
        private List<string> _domainEventTypes;

        public OrdersSubscriber(IEventStoreConnection connection, IMemoryCache cache)
        {
            _connection = connection;
            _cache = cache;
            _domainEventTypes = new List<string>();

            SetDomainEventTypes();
        }

        private void SetDomainEventTypes()
        {
            var type = typeof(IDomainEvent);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            _domainEventTypes.AddRange(types.Select(x => x.Name));
        }

        public async void Subscribe()
        {
            await _connection.ConnectAsync();

            await _connection.SubscribeToAllAsync(true, ProcessEvents);

            var allEvents = new List<ResolvedEvent>();

            AllEventsSlice currentSlice;
            var nextSliceStart = Position.Start;

            do
            {
                currentSlice =
                    _connection.ReadAllEventsForwardAsync(nextSliceStart, 200, false).Result;

                nextSliceStart = currentSlice.NextPosition;

                allEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);
            
            foreach (var evt in allEvents)
            {
                await ProcessEvents(null, evt);
            }
        }

        private Task ProcessEvents(EventStoreSubscription ess, ResolvedEvent evt)
        {
            if (!IsValidEvent(evt))
            {
                return Task.CompletedTask;
            }

            var id = new Guid(evt.OriginalStreamId.Split("_")[1]);

            List<object> events = new List<object>();
            if (!_cache.TryGetValue(id, out events))
            {
                events = new List<object>
                {
                    evt.Event.ToString()
                };
            }

            _cache.Set(id, events);

            return Task.CompletedTask;
        }

        private bool IsValidEvent(ResolvedEvent evt)
        {
            return _domainEventTypes.Contains(evt.Event.EventType);
        }
    }
}
