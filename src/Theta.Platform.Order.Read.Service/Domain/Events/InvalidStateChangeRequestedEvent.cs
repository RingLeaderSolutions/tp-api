using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Read.Service.Domain.Events
{
    public class InvalidStateChangeRequestedEvent : Event
    {
        public InvalidStateChangeRequestedEvent(Guid orderId, string eventType, string reason)
			: base(orderId)
        {
            Reason = reason;
            EventType = eventType;
        }

        public Guid OrderId => AggregateId;

        public string Reason { get; }

        public string EventType { get; }
    }
}
