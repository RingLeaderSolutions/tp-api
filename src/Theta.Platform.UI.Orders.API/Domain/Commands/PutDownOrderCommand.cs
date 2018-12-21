// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.UI.Orders.API.Domain.Commands
{
    public sealed class PutDownOrderCommand : Command
    {
	    public PutDownOrderCommand(Guid orderId)
	    {
		    OrderId = orderId;
	    }

	    public Guid OrderId { get; }
    }
}
