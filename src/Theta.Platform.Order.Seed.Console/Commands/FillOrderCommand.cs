using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public class FillOrderCommand : Command
    {
        public Guid OrderId { get; set; }

        public Guid RFQId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
