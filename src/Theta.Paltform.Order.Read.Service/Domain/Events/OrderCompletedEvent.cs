using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class OrderCompletedEvent : IDomainEvent
    {
        public OrderCompletedEvent(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}
