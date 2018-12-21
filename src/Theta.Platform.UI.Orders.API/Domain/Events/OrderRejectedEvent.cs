// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.UI.Orders.API.Domain.Events
{
    public sealed class OrderRejectedEvent : Event
    {
        public OrderRejectedEvent(Guid orderId, string reason) : base(orderId)
        {
            Reason = reason;
        }

		public Guid OrderId => AggregateId;

		public string Reason { get; set; }
    }
}
