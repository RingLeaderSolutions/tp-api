using Microsoft.Azure.ServiceBus;

namespace Theta.Platform.Messaging.ServiceBus.Factories
{
	public interface IQueueClientFactory
	{
		IQueueClient Create(string queueName);
	}
}