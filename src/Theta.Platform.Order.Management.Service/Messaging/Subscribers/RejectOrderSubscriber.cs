using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Domain.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class RejectOrderSubscriber : Subscriber<RejectOrderCommand, OrderRejectedEvent>, ISubscriber<RejectOrderCommand, OrderRejectedEvent>
    {
        public RejectOrderSubscriber(IAggregateWriter<Domain.Aggregate.Order> aggregateWriter) : base(aggregateWriter)
        {
        }

        protected override async Task<OrderRejectedEvent> Handle(RejectOrderCommand command)
        {
            var order = AggregateWriter.GetById(command.OrderId);

            // Check order state

            var orderRejectdEvent = new OrderRejectedEvent(command.OrderId, command.Reason);

            return orderRejectdEvent;
        }
    }
}
