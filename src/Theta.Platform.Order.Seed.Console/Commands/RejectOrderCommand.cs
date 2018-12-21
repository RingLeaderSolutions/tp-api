// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public sealed class RejectOrderCommand : Command
    {
	    public RejectOrderCommand(Guid orderId, string reason)
	    {
		    OrderId = orderId;
		    Reason = reason;
	    }

	    public Guid OrderId { get; }

        public string Reason { get; }
    }
}
