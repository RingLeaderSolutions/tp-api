﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Messaging.Events
{
    public class OrderPickedUpEvent : IEvent
    {
        public OrderPickedUpEvent(Guid orderId, Guid ownerId)
        {
            OrderId = orderId;
            OwnerId = ownerId;
        }

        public Guid OrderId { get; }

        public Guid OwnerId { get; }

        public Guid AggregateId => OrderId;

        public string Type => this.GetType().Name;
    }
}
