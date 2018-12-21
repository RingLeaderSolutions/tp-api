using System;
using System.Collections.Generic;
using System.Linq;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Order.Management.Service.Domain.Events;

namespace Theta.Platform.Order.Management.Service.Domain
{
	public sealed class OrderAggregateWriter : AggregateWriter<Aggregate.Order>
	{
		public OrderAggregateWriter(
			IEventPersistenceClient eventPersistenceClient, 
			IEventStreamingClient eventStreamingClient) : 
			base(eventPersistenceClient, eventStreamingClient)
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