// ReSharper disable UnusedMember.Global
using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.RFQ.Management.Service.Domain.Events
{
    public sealed class RFQCancelledEvent : Event
    {
        public RFQCancelledEvent(Guid rFQIdentitier) : base(rFQIdentitier)
        {   
        }

        public Guid RFQIdentitier => AggregateId;
    }
}
