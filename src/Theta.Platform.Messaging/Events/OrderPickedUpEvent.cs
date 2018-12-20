using System;

namespace Theta.Platform.Messaging.Events
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
