// ReSharper disable ClassNeverInstantiated.Global
using System;
using System.Collections.Generic;
using System.Linq;
using Theta.Platform.Domain;
using Theta.Platform.RFQ.Management.Service.Domain.Events;

namespace Theta.Platform.RFQ.Management.Service.Domain
{
    public class RequestForQuotes : AggregateRoot
    {
        private RequestForQuotes()
        {
            Register<RFQRaisedEvent>(When);
            //Register<RFQCancelledEvent>(When);
            Register<RFQQuoteReceivedEvent>(When);
            Register<RFQQuoteRetractedEvent>(When);
        }

        public Guid Instrument { get; set; }

        public Guid RFQIdentitier { get; set; }

        public List<string> CounterParties { get; set; }

        public DateTimeOffset Requested { get; set; }

        public List<RFQQuote> RFQQuotes { get; set; }

        public void When(RFQRaisedEvent evt)
        {
            Instrument = evt.Instrument;
            Id = evt.RFQIdentitier;
            CounterParties = evt.CounterParties;
            Requested = evt.Requested;
            RFQQuotes = new List<RFQQuote>();
        }

        private void When(RFQQuoteReceivedEvent evt)
        {
            RFQQuotes.Add(new RFQQuote(evt.QuoteIdentifier, evt.CounterParty, evt.ValidUntil, evt.Price));
        }

        private void When(RFQQuoteRetractedEvent evt)
        {
            var quote = RFQQuotes.SingleOrDefault(r => r.QuoteIdentifier == evt.QuoteIdentifier);

            if (quote != null)
                RFQQuotes.Remove(quote);
        }
    }
}
