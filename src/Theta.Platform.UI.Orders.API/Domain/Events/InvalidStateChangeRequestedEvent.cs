// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.UI.Orders.API.Domain.Events
{
    public sealed class InvalidStateChangeRequestedEvent : Event
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
