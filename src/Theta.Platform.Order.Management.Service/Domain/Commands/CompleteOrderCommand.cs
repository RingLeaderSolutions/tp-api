using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Management.Service.Domain.Commands
{
    public class CompleteOrderCommand : Command
    {
        public Guid OrderId { get; set; }

    }
}
