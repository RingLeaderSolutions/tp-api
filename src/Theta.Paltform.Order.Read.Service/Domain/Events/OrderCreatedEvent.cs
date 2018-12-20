using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class OrderCreatedEvent : Event
    {
        public OrderCreatedEvent(
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
            decimal markupValue) : base (orderId)
        {
            DeskId = deskId;
            ParentOrderId = parentOrderId;
            InstrumentId = instrumentId;
            OwnerId = ownerId;
            Quantity = quantity;
            OrderType = orderType;
            LimitPrice = limitPrice;
            CurrencyCode = currencyCode;
            MarkupUnit = markupUnit;
            MarkupValue = markupValue;
        }

        public Guid DeskId { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

		public Guid OrderId => AggregateId;

		public decimal Quantity { get; set; }

		// TODO: Change this back to use the OrderType enum when moved out of the Theta.Platform.Messaging project
        public string OrderType { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

		// TODO: Change this back to use the MarkupUnit enum when moved out of the Theta.Platform.Messaging project
		public string MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }
    }
}
