// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
using System;
using System.Collections.Generic;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.RFQ.Management.Service.Domain.Events
{
    public sealed class RFQRaisedEvent : Event
    {
        public RFQRaisedEvent(Guid instrument, Guid rFQIdentitier, List<string> counterParties, DateTimeOffset requested) : base(rFQIdentitier)
        {
            CounterParties = counterParties;
            Instrument = instrument;
            Requested = requested;
        }

        public Guid Instrument { get; set; }

        public Guid RFQIdentitier => AggregateId;

        public DateTimeOffset Requested { get; set; }

        public List<string> CounterParties { get; set; }
    }
}
