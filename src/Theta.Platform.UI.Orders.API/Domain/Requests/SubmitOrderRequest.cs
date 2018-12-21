using System;

namespace Theta.Platform.UI.Orders.API.Domain.Requests
{
	public sealed class SubmitOrderRequest
	{
		public Guid Instrument { get; set; }

		public decimal Quantity { get; set; }

		public OrderType Type { get; set; }

		public decimal LimitPrice { get; set; }
		
		public OrderMarkup Markup { get; set; }

		public Side Side { get; set; }

		public Guid Client { get; set; }

		public OrderExpiry Expiry { get; set; }
	}

	public class OrderMarkup
	{
		public MarkupUnit Unit { get; set; }

		public decimal Value { get; set; }
	}

	public class OrderExpiry
	{
		public TimeInForce Type { get; set; }

		public DateTime? GoodTillDate { get; set; }
	}
}