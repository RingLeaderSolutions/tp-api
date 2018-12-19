using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Events;

namespace Theta.Paltform.Order.Read.Service.Domain
{
    public class Order : AggregateRoot
    {
        public Order(
            Guid deskId,
            Guid orderId,
            Guid? parentOrderId,
            Guid instrumentId,
            Guid ownerId,
            decimal quantity,
            OrderType type,
            decimal limitPrice,
            string currencyCode,
            MarkupUnit markupUnit,
            decimal markupValue) : this()
        {
            Raise(new OrderCreatedEvent(
                deskId,
                orderId,
                parentOrderId,
                instrumentId,
                ownerId,
                quantity,
                type.ToString(),
                limitPrice,
                currencyCode,
                markupUnit.ToString(),
                markupValue));
        }

        private Order()
        {
            Register<OrderCreatedEvent>(When);
            Register<OrderCompletedEvent>(When);
            Register<OrderRejectedEvent>(When);
            Register<OrderPickedUpEvent>(When);
            Register<OrderPutDownEvent>(When);
            Register<OrderFilledEvent>(When);
            Register<SupplementaryEvidenceReceivedEvent>(When);
            Register<InvalidStateChangeRequestedEvent>(When);
        }

        public Guid DeskId { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

        public OrderStatus Status { get; private set; }

        public decimal Quantity { get; set; }

        public OrderType OrderType { get; set; }

        public decimal LimitPrice { get; set; }

        public string CurrencyCode { get; set; }

        public MarkupUnit MarkupUnit { get; set; }

        public decimal MarkupValue { get; set; }

        public string SupplementaryEvidence { get; set; }

        public decimal OustandingQuantity
        {
            get
            {
                return Quantity - Fills.Sum(f => f.Quantity);
            }
        }

        public List<Fill> Fills { get; set; }

        public void RecordSupplementaryEvidence(string supplementaryEvidence)
        {
            Raise(new SupplementaryEvidenceReceivedEvent(Id, supplementaryEvidence));
        }

        public void Complete()
        {
            Raise(new OrderCompletedEvent(Id));
        }

        public void Reject(string reason)
        {
            Raise(new OrderRejectedEvent(Id, reason));
        }

        public void Pickup(Guid ownerId)
        {
            Raise(new OrderPickedUpEvent(Id, ownerId));
        }

        public void Fill(Guid orderId, Guid rFQId, decimal price, decimal quantity)
        {
            Raise(new OrderFilledEvent(orderId, rFQId, price, quantity));
        }

        public void RejectPickup(Guid ownerId, string reason)
        {
            Raise(new OrderPickUpRejectedEvent(Id, ownerId, reason));
        }

        public void PutDown()
        {
            Raise(new OrderPutDownEvent(Id));
        }

        public void RaiseInvalidRequestEvent(string eventType, string exception)
        {
            Raise(new InvalidStateChangeRequestedEvent(Id, eventType, exception));
        }

        public void When(OrderCreatedEvent evt)
        {
            DeskId = evt.DeskId;
            Id = evt.OrderId;
            ParentOrderId = evt.ParentOrderId;
            InstrumentId = evt.InstrumentId;
            OwnerId = evt.OwnerId;
            Status = OrderStatus.Pending;
            Quantity = evt.Quantity;
            OrderType = (OrderType)Enum.Parse(typeof(OrderType), evt.OrderType);
            LimitPrice = evt.LimitPrice;
            CurrencyCode = evt.CurrencyCode;
            MarkupUnit = (MarkupUnit)Enum.Parse(typeof(MarkupUnit), evt.MarkupUnit);
            MarkupValue = evt.MarkupValue;
            Fills = new List<Fill>();
        }

        private void When(SupplementaryEvidenceReceivedEvent evt)
        {
            SupplementaryEvidence = evt.SupplementaryEvidence;
        }

        private void When(InvalidStateChangeRequestedEvent evt)
        {
            // Log?
        }

        private void When(OrderCompletedEvent evt)
        {
            Status = OrderStatus.Done;
        }

        private void When(OrderFilledEvent evt)
        {
            if (evt.Quantity < OustandingQuantity)
            {
                Status = OrderStatus.PartiallyFilled;
            }
            else
            {
                Status = OrderStatus.Filled;
            }

            Fills.Add(new Domain.Fill(evt.RFQId, evt.Price, evt.Quantity));
        }

        private void When(OrderRejectedEvent evt)
        {
            Status = OrderStatus.Rejected;
        }

        private void When(OrderPickedUpEvent evt)
        {
            OwnerId = evt.OwnerId;
            Status = OrderStatus.Working;
        }

        private void When(OrderPutDownEvent evt)
        {
            Status = OrderStatus.Pending;
        }
    }
}
