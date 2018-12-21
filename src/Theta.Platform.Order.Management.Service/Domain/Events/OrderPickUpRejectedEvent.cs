// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Domain.Events
{
    public sealed class OrderPickUpRejectedEvent : Event
    {
        public OrderPickUpRejectedEvent(Guid orderId, Guid ownerId, string reason)
			:base(orderId)
        {
            OwnerId = ownerId;
            Reason = reason;
        }

        public Guid OrderId => AggregateId;

        public Guid OwnerId { get; set; }

        public string Reason { get; set; }
    }
}
