using System;

namespace Theta.Platform.UI.Orders.API.Domain.Requests
{
	public sealed class SubmitOrderResponse
	{
		public SubmitOrderResponse(Guid orderId)
		{
			OrderId = orderId;
		}

		public Guid OrderId { get; set; }
	}
}