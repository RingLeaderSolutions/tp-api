using System;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public abstract class Subscriber<TCommand, TEvent> : ISubscriber<ICommand, IEvent> 
	    where TCommand : ICommand where TEvent : IEvent
    {
        protected readonly IAggregateWriter<Domain.Order> AggregateWriter;

        protected Subscriber(IAggregateWriter<Domain.Order> aggregateWriter)
        {
            AggregateWriter = aggregateWriter;
        }

        public Type CommandType => typeof(TCommand);

        public async Task HandleCommand(IActionableMessage<ICommand> command)
        {
            try
            {
                var evt = await Handle((TCommand)command.ReceivedCommand);

                // TODO: Atomicity a bit lost here - saving of event and completion of queue message should be atomic
                await AggregateWriter.Save(evt);
            }
            catch (Exception ex)
            {
                // TODO: Log here
                await command.Reject("Exception", ex.Message);
            }

            await command.Complete();
        }

        protected abstract Task<TEvent> Handle(TCommand command);
    }
}