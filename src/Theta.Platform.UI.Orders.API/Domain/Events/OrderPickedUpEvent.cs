using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.UI.Orders.API.Domain.Events
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
