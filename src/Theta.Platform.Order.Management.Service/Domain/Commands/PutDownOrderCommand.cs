using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Management.Service.Domain.Commands
{
    public class PutDownOrderCommand : Command
    {
        public Guid OrderId { get; set; }
    }
}
