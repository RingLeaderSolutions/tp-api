﻿// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Order.Management.Service.Domain.Aggregate;

namespace Theta.Platform.Order.Management.Service.Domain.Commands
{
    public sealed class CreateOrderCommand : Command
    {
        public Guid DeskId { get; set; }

        public Guid? ParentOrderId { get; set; }

        public Guid InstrumentId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid OrderId { get; set; }

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
