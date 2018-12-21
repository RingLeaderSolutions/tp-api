// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Order.Seed.Console.Domain;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public sealed class CreateOrderCommand : Command
    {
		// TODO: Reinstate enums when commands moved out of Theta.Platform.Messaging project.
        public CreateOrderCommand(
                    Guid deskId,
                    Guid orderId,
                    Guid? parentOrderId,
                    Guid instrumentId,
                    Guid ownerId,
                    decimal quantity,
					Side side,
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
            Side = side;
            OrderType = orderType;
            LimitPrice = limitPrice;
            CurrencyCode = currencyCode;
            MarkupUnit = markupUnit;
            MarkupValue = markupValue;
            GoodTillDate = goodTillDate;
            TimeInForce = timeInForce;
        }

        public Guid DeskId { get; }

        public Guid? ParentOrderId { get; }

        public Guid InstrumentId { get; }

        public Guid OwnerId { get; }

        public Guid OrderId { get; }

        public decimal Quantity { get; }

		public Side Side { get; }

        public string OrderType { get; }

        public decimal LimitPrice { get; }

        public string CurrencyCode { get; }

        public string MarkupUnit { get; }

        public decimal MarkupValue { get; }

        public DateTime? GoodTillDate { get; }

        public string TimeInForce { get; }
	}
}
