using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Messaging.Events
{
    public class OrderPickUpRejectedEvent : IEvent
    {
        public OrderPickUpRejectedEvent(Guid orderId, Guid ownerId, string reason)
        {
            OrderId = orderId;
            OwnerId = ownerId;
            Reason = reason;
        }

        public Guid OrderId { get; }

        public Guid OwnerId { get; }

        public string Reason { get; }

        public Guid AggregateId => OrderId;

        public string Type => this.GetType().Name;
    }
}
