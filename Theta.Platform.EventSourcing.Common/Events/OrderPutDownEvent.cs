using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Messaging.Events
{
    public class OrderPutDownEvent : IEvent
    {
        public OrderPutDownEvent(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }

        public Guid AggregateId => OrderId;

        public string Type => this.GetType().Name;
    }
}
