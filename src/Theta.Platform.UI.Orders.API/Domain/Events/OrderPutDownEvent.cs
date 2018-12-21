﻿using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.UI.Orders.API.Domain.Events
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