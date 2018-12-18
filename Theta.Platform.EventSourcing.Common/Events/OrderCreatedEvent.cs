using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Messaging.Events
{
    public class OrderCreatedEvent : IEvent
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
            decimal markupValue)
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

        public Guid AggregateId => OrderId;

        public string Type => this.GetType().Name;
    }
}
