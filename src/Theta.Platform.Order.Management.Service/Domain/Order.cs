using System;
using Theta.Platform.EventStore;
using Theta.Platform.Order.Management.Service.Domain.Events;

namespace Theta.Platform.Order.Management.Service
{
    public class Order : AggregateBase
    {
        public Order(
            Guid deskId,
            Guid orderId,
            Guid? parentOrderId,
            Guid instrumentId,
            Guid ownerId,
            OrderStatus status,
            decimal quantity,
            OrderType type,
            decimal limitPrice,
            string currencyCode,
            MarkupUnit markupUnit,
            decimal markupValue)
        {
            RaiseEvent(new OrderCreatedEvent(
                deskId,
                orderId,
                parentOrderId,
                instrumentId,
                ownerId,
                quantity,
                type,
                limitPrice,
                currencyCode,
                markupUnit,
                markupValue));
        }

        public Guid DeskId { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid OrderId { get; set; }

        public OrderStatus Status { get; private set; }

        public decimal Quantity { get; set; }

        public OrderType Type { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public MarkupUnit MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }

        public override object Identifier => $"trade-{OrderId}";

        public void Apply(OrderCreatedEvent evt)
        {
            DeskId = evt.DeskId;
            OrderId = evt.OrderId;
            ParentOrderId = evt.ParentOrderId;
            InstrumentId = evt.InstrumentId;
            OwnerId = evt.OwnerId;
            Status = OrderStatus.Pending;
            Quantity = evt.Quantity;
            Type = evt.Type;
            LimitPrice = evt.LimitPrice;
            CurrencyCode = evt.CurrencyCode;
            MarkupUnit = evt.MarkupUnit;
            MarkupValue = evt.MarkupValue;
        }

        public void RecordSupplementaryEvidence(string supplementaryEvidence)
        {
            RaiseEvent(new SupplementaryEvidenceReceivedEvent(OrderId, supplementaryEvidence));
        }

        public void Reject(string reason)
        {
            RaiseEvent(new OrderRejectedEvent(OrderId, reason));
        }

        public void Complete()
        {
            RaiseEvent(new OrderCompletedEvent(OrderId));
        }
    }
}
