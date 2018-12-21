using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.UI.Orders.API.Domain.Commands
{
    public class PickupOrderCommand : Command
    {
	    public PickupOrderCommand(Guid orderId, Guid ownerId)
	    {
		    OrderId = orderId;
		    OwnerId = ownerId;
	    }

	    public Guid OrderId { get; set; }

        public Guid OwnerId { get; set; }
    }
}
