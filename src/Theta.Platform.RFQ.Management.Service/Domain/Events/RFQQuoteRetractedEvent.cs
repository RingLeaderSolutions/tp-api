using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.RFQ.Management.Service.Domain.Events
{
    public class RFQQuoteRetractedEvent : Event
    {
        public RFQQuoteRetractedEvent(Guid rFQIdentitier, Guid quoteIdentifier) : base(rFQIdentitier)
        {
            QuoteIdentifier = quoteIdentifier;
        }

        public Guid QuoteIdentifier { get; set; }
    }
}
