using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
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
