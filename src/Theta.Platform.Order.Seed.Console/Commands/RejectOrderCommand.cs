using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public class RejectOrderCommand : Command
    {
        public Guid OrderId { get; set; }

        public string Reason { get; set; }
    }
}
