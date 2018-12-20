using System;

namespace Theta.Platform.Messaging.Events
{
    public class OrderFilledEvent : Event
    {
        public OrderFilledEvent(Guid orderId, Guid rFQId, decimal price, decimal quantity)
			: base(orderId)
        {
            RFQId = rFQId;
            Price = price;
            Quantity = quantity;
        }

        public Guid OrderId => AggregateId;

		public Guid RFQId { get; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }
    }
}
