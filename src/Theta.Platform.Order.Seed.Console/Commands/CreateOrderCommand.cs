// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Order.Seed.Console.Domain;

namespace Theta.Platform.Order.Seed.Console.Commands
{
    public sealed class CreateOrderCommand : Command
    {
        public CreateOrderCommand(
                    Guid deskId,
                    Guid orderId,
                    Guid? parentOrderId,
                    Guid instrumentId,
                    Guid ownerId,
                    decimal quantity,
					Side side,
                    OrderType orderType,
                    decimal limitPrice,
                    string currencyCode,
                    MarkupUnit markupUnit,
                    decimal markupValue,
                    DateTimeOffset? goodTillDate,
                    TimeInForce timeInForce)
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

        public OrderType OrderType { get; }

        public decimal LimitPrice { get; }

        public string CurrencyCode { get; }

        public MarkupUnit MarkupUnit { get; }

        public decimal MarkupValue { get; }

        public DateTimeOffset? GoodTillDate { get; }

        public TimeInForce TimeInForce { get; }
	}
}
