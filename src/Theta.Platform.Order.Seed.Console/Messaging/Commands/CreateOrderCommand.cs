using System;
using System.Collections.Generic;
using System.Text;

namespace Theta.Platform.Order.Seed.Console.Messaging.Commands
{
    public class CreateOrderCommand
    {
        public Guid DeskId { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid OrderId { get; set; }

        public decimal Quantity { get; set; }

        public string Type { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public string MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }

        public DateTime? GoodTillDate { get; set; }

        public string TimeInForce { get; set; }
    }
}
