using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class OrderRejectedEvent : Event
    {
        public OrderRejectedEvent(Guid orderId, string reason) : base(orderId)
        {
            Reason = reason;
        }

		public Guid OrderId => AggregateId;

		public string Reason { get; }
    }
}
