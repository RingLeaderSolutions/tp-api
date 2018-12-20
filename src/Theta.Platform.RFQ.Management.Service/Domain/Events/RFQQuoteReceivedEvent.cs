using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.RFQ.Management.Service.Domain.Events
{
    public class RFQQuoteReceivedEvent : Event
    {
        public RFQQuoteReceivedEvent(Guid instrument, Guid rFQIdentitier, Guid quoteIdentifier, string counterParty, DateTimeOffset validUntil, decimal price) : base(rFQIdentitier)
        {
            CounterParty = counterParty;
            Instrument = instrument;
            ValidUntil = validUntil;
            Price = price;
            QuoteIdentifier = quoteIdentifier;
        }

        public Guid Instrument { get; set; }

        public Guid RFQIdentitier => AggregateId;

        public string CounterParty { get; set; }

        public DateTimeOffset ValidUntil { get; set; }

        public decimal Price { get; set; }

        public Guid QuoteIdentifier { get; set; }
    }
}
