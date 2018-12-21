using System;
using System.Collections.Generic;
using System.Linq;
using Theta.Platform.Domain;
using Theta.Platform.Order.Management.Service.Domain.Events;

namespace Theta.Platform.Order.Management.Service.Domain
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public sealed class Order : AggregateRoot
    {
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

		public Side Side { get; set; }

        public OrderType Type { get; set; }

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

		public void When(OrderCreatedEvent evt)
		{
			DeskId = evt.DeskId;
			Id = evt.OrderId;
			ParentOrderId = evt.ParentOrderId;
			InstrumentId = evt.InstrumentId;
			OwnerId = evt.OwnerId;
			Status = OrderStatus.Pending;
			Quantity = evt.Quantity;
			Side = evt.Side;
			// TODO: Remove enum parsing when OrderCreatedEvent moved out of the Theta.Platform.Messaging project
			Type = Enum.Parse<OrderType>(evt.OrderType);
			LimitPrice = evt.LimitPrice;
			CurrencyCode = evt.CurrencyCode;
			// TODO: Remove enum parsing when OrderCreatedEvent moved out of the Theta.Platform.Messaging project
			MarkupUnit = Enum.Parse<MarkupUnit>(evt.MarkupUnit);
			MarkupValue = evt.MarkupValue;
			Fills = new List<Fill>();
		}

		private void When(SupplementaryEvidenceReceivedEvent evt)
		{
			SupplementaryEvidence = evt.SupplementaryEvidence;
		}

		private void When(InvalidStateChangeRequestedEvent evt)
		{
			// TODO: Log?
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

			Fills.Add(new Fill(evt.RFQId, evt.Price, evt.Quantity));
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
