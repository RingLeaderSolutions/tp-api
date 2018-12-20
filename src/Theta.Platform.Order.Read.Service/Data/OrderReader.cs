using System;
using System.Collections.Generic;
using System.Linq;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Order.Read.Service.Domain.Events;

namespace Theta.Platform.Order.Read.Service.Data
{
	public class OrderReader : AggregateReader<Domain.Order>, IOrderReader
    {
        public OrderReader(
	        IEventPersistenceClient eventPersistenceClient, 
	        IEventStreamingClient eventStreamingClient) 
            : base(eventPersistenceClient, eventStreamingClient)
        {
        }

		protected override Dictionary<string, Type> SubscribedEventTypes { get; } = new List<KeyValuePair<string, Type>>
        {
	        CreateEventNameToTypeMapping(typeof(OrderCreatedEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderCompletedEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderFilledEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderPickedUpEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderPickUpRejectedEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderPutDownEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderRejectedEvent)),
	        CreateEventNameToTypeMapping(typeof(SupplementaryEvidenceReceivedEvent))
        }.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        private static KeyValuePair<string, Type> CreateEventNameToTypeMapping(Type type)
        {
	        return new KeyValuePair<string, Type>(type.Name, type);
        }
	}
}
