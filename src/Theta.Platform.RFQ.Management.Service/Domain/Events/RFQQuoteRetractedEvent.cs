// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.RFQ.Management.Service.Domain.Events
{
    public sealed class RFQQuoteRetractedEvent : Event
    {
        public RFQQuoteRetractedEvent(Guid rFQIdentitier, Guid quoteIdentifier) : base(rFQIdentitier)
        {
            QuoteIdentifier = quoteIdentifier;
        }

        public Guid QuoteIdentifier { get; set; }
    }
}
