using System;
using System.Collections.Generic;
using System.Text;

namespace Theta.Platform.Order.Seed.Console.Messaging.Commands
{
    public class PickupOrderCommand
    {
        public Guid OrderId { get; set; }

        public Guid OwnerId { get; set; }
    }
}
