using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class OrderRejectedEvent : IDomainEvent
    {
        public OrderRejectedEvent(Guid orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }

        public Guid OrderId { get; }
        public string Reason { get; }
    }
}
