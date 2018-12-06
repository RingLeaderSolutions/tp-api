using System;
using Theta.Platform.Domain.Instruments;

namespace Theta.Platform.Domain.Order
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Instrument Instrument { get; set; }

        public OrderStatus Status { get; set; }

        public Side Side { get; set; }

        public decimal Quantity { get; set; }

        public OrderType Type { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public MarkupUnit MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }
    }
}
