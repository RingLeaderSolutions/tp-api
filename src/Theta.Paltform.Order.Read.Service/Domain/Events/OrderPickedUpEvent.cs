using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class OrderPickedUpEvent : Event
    {
        public OrderPickedUpEvent(Guid orderId, Guid ownerId) : 
	        base(orderId)
        {
            OwnerId = ownerId;
        }

        public Guid OrderId => AggregateId;

        public Guid OwnerId { get; }
    }
}
