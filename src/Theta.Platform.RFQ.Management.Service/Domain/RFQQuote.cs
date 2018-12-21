using System;

namespace Theta.Platform.RFQ.Management.Service.Domain
{
    public class RFQQuote
    {
        public RFQQuote(Guid quoteIdentifier, string counterParty, DateTimeOffset validUntil, decimal price)
        {
            QuoteIdentifier = quoteIdentifier;
            CounterParty = counterParty;
            ValidUntil = validUntil;
            Price = price;
        }

        public string CounterParty { get; set; }

        public DateTimeOffset ValidUntil { get; set; }

        public decimal Price { get; set; }

        public Guid QuoteIdentifier { get; set; }
    }
}
