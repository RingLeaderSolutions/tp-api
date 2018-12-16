using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Domain.Events
{
    public class OrderFilledEvent
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
