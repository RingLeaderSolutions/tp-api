// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public sealed class FillOrderCommand : Command
    {
	    public FillOrderCommand(Guid orderId, Guid rfqId, decimal quantity, decimal price)
	    {
		    OrderId = orderId;
		    RFQId = rfqId;
		    Quantity = quantity;
		    Price = price;
	    }

	    public Guid OrderId { get; }

        public Guid RFQId { get; }

        public decimal Quantity { get; }

        public decimal Price { get; }
    }
}
