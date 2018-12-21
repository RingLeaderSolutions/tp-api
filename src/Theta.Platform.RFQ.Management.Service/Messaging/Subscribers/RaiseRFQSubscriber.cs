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
    public class RaiseRFQSubscriber : Subscriber<RaiseRFQCommand, RFQRaisedEvent>, ISubscriber<RaiseRFQCommand, RFQRaisedEvent>
    {
        public RaiseRFQSubscriber(IAggregateWriter<RequestForQuotes> aggregateWriter, IQuoteProvider quoteProvider) 
            : base(aggregateWriter, quoteProvider)
        {
        }

        protected override async Task<RFQRaisedEvent> Handle(RaiseRFQCommand command)
        {
            var rfq = AggregateWriter.GetById(command.RFQIdentitier);

            if (rfq != null)
            {
                // already processing, what to do here?
            }

            var rFQRaisedEvent = new RFQRaisedEvent(command.Instrument, command.RFQIdentitier, command.CounterParties, command.Requested);

            // Save the creation
            await base.SaveEvent(rFQRaisedEvent);

            //  and then SubmitRequest
            QuoteProvider.SubmitRFQ(command.RFQIdentitier, command.OrderId, command.CounterParties, command.Instrument, 110.25M);

            return rFQRaisedEvent;
        }

        protected override Task SaveEvent(RFQRaisedEvent evt)
        {
            return Task.CompletedTask;
        }
    }
}
