using System;

namespace Theta.Platform.Messaging.Events
{
    public class OrderPutDownEvent : Event
    {
        public OrderPutDownEvent(Guid orderId) 
	        : base(orderId)
        {
        }

		public Guid OrderId => AggregateId;
	}
}
