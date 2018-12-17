using System;
using System.Collections.Generic;
using System.Text;

namespace Theta.Platform.Order.Seed.Console.Messaging.Commands
{
    public class FillOrderCommand
    {
        public Guid OrderId { get; set; }

        public Guid RFQId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
