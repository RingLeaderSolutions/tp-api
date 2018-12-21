// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public sealed class PickupOrderCommand : Command
    {
	    public PickupOrderCommand(Guid orderId, Guid ownerId)
	    {
		    OrderId = orderId;
		    OwnerId = ownerId;
	    }

	    public Guid OrderId { get; }

        public Guid OwnerId { get; }
    }
}
