﻿using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Domain.Events
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