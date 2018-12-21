using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Order.Management.Service.Domain;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Domain.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class CompleteOrderSubscriber : Subscriber<CompleteOrderCommand, OrderCompletedEvent>, ISubscriber<CompleteOrderCommand, OrderCompletedEvent>
    {
        public CompleteOrderSubscriber(IAggregateWriter<Domain.Aggregate.Order> aggregateWriter) : base(aggregateWriter)
        {
        }

        protected override async Task<OrderCompletedEvent> Handle(CompleteOrderCommand command)
        {
            var order = AggregateWriter.GetById(command.OrderId);

            var orderCompletedEvent = new OrderCompletedEvent(order.Id);

            return orderCompletedEvent;
        }
    }
}
