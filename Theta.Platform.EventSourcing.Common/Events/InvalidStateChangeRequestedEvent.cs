using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Messaging.Events
{
    public class InvalidStateChangeRequestedEvent : IEvent
    {
        public InvalidStateChangeRequestedEvent(Guid orderId, string eventType, string reason)
        {
            OrderId = orderId;

            Reason = reason;

            EventType = eventType;
        }

        public Guid OrderId { get; }

        public string Reason { get; }

        public string EventType { get; }

        public Guid AggregateId => OrderId;

        public string Type => this.GetType().Name;
    }
}
