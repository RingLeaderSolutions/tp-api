using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.RFQ.Management.Service.Domain;
using Theta.Platform.RFQ.Management.Service.Domain.Commands;
using Theta.Platform.RFQ.Management.Service.Domain.Events;
using Theta.Platform.RFQ.Management.Service.QuoteManagement;

namespace Theta.Platform.RFQ.Management.Service.Messaging.Subscribers
{
    public class CancelRFQRequestSubscriber : Subscriber<CancelRFQCommand, RFQCancelledEvent>, ISubscriber<RaiseRFQCommand, RFQRaisedEvent>
    {
        public CancelRFQRequestSubscriber(IAggregateWriter<RequestForQuotes> aggregateWriter, IQuoteProvider quoteProvider)
            : base(aggregateWriter, quoteProvider)
        {
        }

        protected override async Task<RFQCancelledEvent> Handle(CancelRFQCommand command)
        {
            var rfq = AggregateWriter.GetById(command.RFQIdentitier);

            if (rfq == null)
            {
                // invlid request what to do here?
            }

            QuoteProvider.CancelRFQ(command.RFQIdentitier);

            var rFQCancelledEvent = new RFQCancelledEvent(command.RFQIdentitier);

            return rFQCancelledEvent;
        }
    }
}
