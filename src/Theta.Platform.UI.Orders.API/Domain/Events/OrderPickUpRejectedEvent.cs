using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.UI.Orders.API.Domain.Events
{
    public class OrderPickUpRejectedEvent : Event
    {
        public OrderPickUpRejectedEvent(Guid orderId, Guid ownerId, string reason)
			:base(orderId)
        {
            OwnerId = ownerId;
            Reason = reason;
        }

        public Guid OrderId => AggregateId;

        public Guid OwnerId { get; }

        public string Reason { get; }
    }
}
