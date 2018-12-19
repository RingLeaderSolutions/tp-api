using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Messaging.Events
{
    public class OrderRejectedEvent : IEvent
    {
        public OrderRejectedEvent(Guid orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }

        public Guid OrderId { get; }

        public string Reason { get; }

        public Guid AggregateId => OrderId;

        public string Type => this.GetType().Name;
    }
}
