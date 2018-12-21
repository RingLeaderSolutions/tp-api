using System;

namespace Theta.Platform.UI.Orders.API.Domain
{
    public class Fill
    {
        public Fill(Guid rFQId, decimal price, decimal quantity)
        {
            RFQId = rFQId;

            Price = price;

            Quantity = quantity;
        }

        public Guid RFQId { get; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }
    }
}
