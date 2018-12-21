using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.RFQ.Management.Service.Domain.Events
{
    public class RFQCancelledEvent : Event
    {
        public RFQCancelledEvent(Guid rFQIdentitier) : base(rFQIdentitier)
        {
            
        }

        public Guid RFQIdentitier => AggregateId;
    }
}
