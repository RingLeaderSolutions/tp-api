using System;

namespace Theta.Platform.Messaging.Events
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
