using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Domain.Events
{
    public class OrderCompletedEvent
    {
        public OrderCompletedEvent(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}
