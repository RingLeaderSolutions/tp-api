// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.UI.Orders.API.Domain.Events
{
    public sealed class OrderPickedUpEvent : Event
    {
        public OrderPickedUpEvent(Guid orderId, Guid ownerId) : 
	        base(orderId)
        {
            OwnerId = ownerId;
        }

        public Guid OrderId => AggregateId;

        public Guid OwnerId { get; set; }
    }
}
