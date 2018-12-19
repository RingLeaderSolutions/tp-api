using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class PutDownOrderSubscriber : Subscriber<PutDownOrderCommand, OrderPutDownEvent>, ISubscriber<PutDownOrderCommand, OrderPutDownEvent>
    {
        public PutDownOrderSubscriber(IAggregateWriter<Domain.Order> aggregateWriter) : base(aggregateWriter)
        {
        }

        protected override async Task<OrderPutDownEvent> Handle(PutDownOrderCommand command)
        {
            var order = AggregateWriter.GetById(command.OrderId);

            // Check order state

            var orderPutDowndEvent = new OrderPutDownEvent(command.OrderId);

            return orderPutDowndEvent;
        }
    }
}
