// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
using System;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Order.Management.Service.Domain.Aggregate;

namespace Theta.Platform.Order.Management.Service.Domain.Events
{
    public sealed class OrderCreatedEvent : Event
    {
        public OrderCreatedEvent(
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
            TimeInForce timeInForce) : base (orderId)
        {
            DeskId = deskId;
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

        public Guid DeskId { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

		public Guid OrderId => AggregateId;

		public decimal Quantity { get; set; }

		public Side Side { get; set; }

        public OrderType OrderType { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

		public MarkupUnit MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }

        public DateTimeOffset? GoodTillDate { get; set; }

        public TimeInForce TimeInForce { get; set; }
	}
}
