using System;
using System.Collections.Generic;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Domain
{
	public class OrderAggregateWriter : AggregateWriter<Order>
	{
		public OrderAggregateWriter(
			IEventPersistenceClient eventPersistenceClient, 
			IEventStreamingClient eventStreamingClient) : 
			base(eventPersistenceClient, eventStreamingClient)
		{
		}

		public override Dictionary<string, Type> GetEventTypes()
		{
			var collection = new Dictionary<string, Type>();

			AddEventType(collection, typeof(OrderCompletedEvent));
			AddEventType(collection, typeof(OrderCreatedEvent));
			AddEventType(collection, typeof(OrderFilledEvent));
			AddEventType(collection, typeof(OrderPickedUpEvent));
			AddEventType(collection, typeof(OrderPickUpRejectedEvent));
			AddEventType(collection, typeof(OrderPutDownEvent));
			AddEventType(collection, typeof(OrderRejectedEvent));
			AddEventType(collection, typeof(SupplementaryEvidenceReceivedEvent));

			return collection;
		}

		private static void AddEventType(Dictionary<string, Type> collection, Type type)
		{
			collection.Add(type.Name, type);
		}
	}
}