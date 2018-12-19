using System;

namespace Theta.Platform.Messaging.Commands
{
    public class CreateOrderCommand : ICommand
    {
        public CreateOrderCommand(
                    Guid deskId,
                    Guid orderId,
                    Guid? parentOrderId,
                    Guid instrumentId,
                    Guid ownerId,
                    decimal quantity,
                    string orderType,
                    decimal limitPrice,
                    string currencyCode,
                    string markupUnit,
                    decimal markupValue,
                    DateTime? goodTillDate,
                    string timeInForce)
        {
            DeskId = deskId;
            OrderId = orderId;
            ParentOrderId = parentOrderId;
            InstrumentId = instrumentId;
            OwnerId = ownerId;
            Quantity = quantity;
            OrderType = orderType;
            LimitPrice = limitPrice;
            CurrencyCode = currencyCode;
            MarkupUnit = markupUnit;
            MarkupValue = markupValue;
            GoodTillDate = goodTillDate;
            TimeInForce = timeInForce;
        }

        public Guid DeskId { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid OrderId { get; set; }

        public decimal Quantity { get; set; }

        public string OrderType { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public string MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }

        public DateTime? GoodTillDate { get; set; }

        public string TimeInForce { get; set; }

		public string Type => this.GetType().Name;
	}
}
