using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public class CompleteOrderCommand : Command
    {
        public Guid OrderId { get; set; }

    }
}
