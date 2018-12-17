using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class OrderFilledEvent : IDomainEvent
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
    }
}
