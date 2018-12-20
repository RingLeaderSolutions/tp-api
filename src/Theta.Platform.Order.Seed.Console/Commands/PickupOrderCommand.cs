using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public class PickupOrderCommand : Command
    {
        public Guid OrderId { get; set; }

        public Guid OwnerId { get; set; }
    }
}
