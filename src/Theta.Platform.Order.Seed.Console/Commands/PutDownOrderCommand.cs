using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public class PutDownOrderCommand : Command
    {
        public Guid OrderId { get; set; }
    }
}
