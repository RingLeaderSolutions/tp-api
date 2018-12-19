using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
	public class CreateOrderSubscriber : Subscriber<CreateOrderCommand>
	{
		private readonly IAggregateWriter<Domain.Order> _aggregateWriter;

		public CreateOrderSubscriber(IAggregateWriter<Domain.Order> aggregateWriter)
		{
			_aggregateWriter = aggregateWriter;
		}

		public override async Task Handle(CreateOrderCommand command)
		{
			//var orderCreatedEvent = new OrderCreatedEvent()

			// whatever logic necessary ? maybe 
			//await _aggregateWriter.Save()
		}
	}
}
