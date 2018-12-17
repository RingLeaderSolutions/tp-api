using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class InvalidStateChangeRequestedEvent : IDomainEvent
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
    }
}
