using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Messaging.Events
{
    public class OrderFilledEvent : IEvent
    {
        public OrderFilledEvent(Guid orderId, Guid rFQId, decimal price, decimal quantity)
        {
            OrderId = orderId;
            RFQId = rFQId;
            Price = price;
            Quantity = quantity;
        }

        public Guid OrderId { get; }

        public Guid RFQId { get; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public Guid AggregateId => OrderId;

        public string Type => this.GetType().Name;
    }
}
