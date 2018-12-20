using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;
using Theta.Platform.RFQ.Management.Service.Domain;
using Theta.Platform.RFQ.Management.Service.QuoteManagement;

namespace Theta.Platform.RFQ.Management.Service.Messaging.Subscribers
{
    public abstract class Subscriber<TCommand, TEvent> : ISubscriber<ICommand, IEvent>
        where TCommand : ICommand where TEvent : IEvent
    {
        protected readonly IAggregateWriter<RequestForQuotes> AggregateWriter;
        protected readonly IQuoteProvider QuoteProvider;

        protected Subscriber(IAggregateWriter<RequestForQuotes> aggregateWriter, IQuoteProvider quoteProvider)
        {
            AggregateWriter = aggregateWriter;
            QuoteProvider = quoteProvider;
        }

        public Type CommandType => typeof(TCommand);

        public async Task HandleCommand(IActionableMessage<ICommand> command)
        {
            try
            {
                var evt = await Handle((TCommand)command.ReceivedCommand);

                await SaveEvent(evt);
            }
            catch (Exception ex)
            {
                // TODO: Log here
                await command.Reject("Exception", ex.Message);
            }

            await command.Complete();
        }

        protected virtual async Task SaveEvent(TEvent evt)
        {
            await AggregateWriter.Save(evt);
        }

        protected abstract Task<TEvent> Handle(TCommand command);
    }
}