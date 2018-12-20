using System;

namespace Theta.Platform.Messaging.Events
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
