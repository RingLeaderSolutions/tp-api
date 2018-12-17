using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service.Domain.Events
{
    public class OrderPickedUpEvent : IDomainEvent
    {
        public OrderPickedUpEvent(Guid orderId, Guid ownerId)
        {
            OrderId = orderId;
            OwnerId = ownerId;
        }

        public Guid OrderId { get; }
        public Guid OwnerId { get; }
    }
}
