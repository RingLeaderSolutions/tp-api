using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class RejectOrderSubscriber : Subscriber<RejectOrderCommand, OrderRejectedEvent>, ISubscriber<RejectOrderCommand, OrderRejectedEvent>
    {
        public RejectOrderSubscriber(IAggregateWriter<Domain.Order> aggregateWriter) : base(aggregateWriter)
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
