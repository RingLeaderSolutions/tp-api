using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Domain.Events
{
    public class OrderPickUpRejectedEvent
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
