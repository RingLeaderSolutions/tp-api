﻿// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.UI.Orders.API.Domain.Events
{
    public sealed class OrderFilledEvent : Event
    {
        public OrderFilledEvent(Guid orderId, Guid rFQId, decimal price, decimal quantity)
			: base(orderId)
        {
            RFQId = rFQId;
            Price = price;
            Quantity = quantity;
        }

        public Guid OrderId => AggregateId;

		public Guid RFQId { get; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }
    }
}
