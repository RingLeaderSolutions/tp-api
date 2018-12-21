// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Management.Service.Domain.Commands
{
    public sealed class CompleteOrderCommand : Command
    {
        public Guid OrderId { get; set; }
    }
}
