using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class PickupOrderSubscriber : Subscriber<PickupOrderCommand, OrderPickedUpEvent>, ISubscriber<PickupOrderCommand, OrderPickedUpEvent>
    {
        public PickupOrderSubscriber(IAggregateWriter<Domain.Order> aggregateWriter) : base(aggregateWriter)
        {
        }

        protected override async Task<OrderPickedUpEvent> Handle(PickupOrderCommand command)
        {
            var order = AggregateWriter.GetById(command.OrderId);

            // Check order state

            var orderPickupdEvent = new OrderPickedUpEvent(command.OrderId, command.OwnerId);

            return orderPickupdEvent;
        }
    }
}
