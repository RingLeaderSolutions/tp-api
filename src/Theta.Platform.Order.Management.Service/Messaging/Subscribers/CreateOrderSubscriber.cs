using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Domain.Events;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class CreateOrderSubscriber : Subscriber<CreateOrderCommand, OrderCreatedEvent>, ISubscriber<CreateOrderCommand, OrderCreatedEvent>
    {
        public CreateOrderSubscriber(IAggregateWriter<Domain.Order> aggregateWriter) : base(aggregateWriter)
        {
        }

        protected override Task<OrderCreatedEvent> Handle(CreateOrderCommand command)
        {
            var orderCreatedEvent = new OrderCreatedEvent(
	            command.DeskId, 
	            command.OrderId, 
	            command.ParentOrderId, 
	            command.InstrumentId, 
	            command.OwnerId, 
	            command.Quantity, 
				command.Side,
	            command.OrderType, 
	            command.LimitPrice, 
	            command.CurrencyCode, 
	            command.MarkupUnit, 
	            command.MarkupValue);

            return Task.FromResult(orderCreatedEvent);
        }
    }
}
