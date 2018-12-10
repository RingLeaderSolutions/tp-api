using System;

namespace Theta.Platform.Order.Management.Service
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid EntityId { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Quantity { get; set; }

        public OrderType Type { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public MarkupUnit MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }
    }
}
