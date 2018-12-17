using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class OrderPickUpRejectedEvent : IDomainEvent
    {
        public OrderPickUpRejectedEvent(Guid orderId, Guid ownerId, string reason)
        {
            OrderId = orderId;
            OwnerId = ownerId;
            Reason = reason;
        }

        public Guid OrderId { get; }
        public Guid OwnerId { get; }
        public string Reason { get; }
    }
}
