using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public abstract class Subscriber<T, V> : ISubscriber<ICommand, IEvent> where T : ICommand where V : IEvent
    {
        protected IAggregateWriter<Domain.Order> AggregateWriter;

        public Subscriber(IAggregateWriter<Domain.Order> aggregateWriter)
        {
            AggregateWriter = aggregateWriter;
        }

        public async Task HandleCommand(IActionableMessage<ICommand> command)
        {
            try
            {
                var evnt = await Handle((T)command.ReceivedCommand);

                // Atomicity a bit lost here 
                await AggregateWriter.Save(evnt);
            }
            catch (Exception ex)
            {
                // Log 
                await command.Reject("Exception", ex.Message);
            }

            await command.Complete();
        }

        protected abstract Task<V> Handle(T command);
    }
}