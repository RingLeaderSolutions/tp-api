using Microsoft.Azure.ServiceBus;
using Theta.Platform.Messaging.ServiceBus.Configuration;

namespace Theta.Platform.Messaging.ServiceBus.Factories
{
	public sealed class QueueClientFactory : IQueueClientFactory
	{
		private readonly string _serviceBusConnectionString;

		public QueueClientFactory(IServiceBusConfiguration serviceBusConfiguration)
		{
			_serviceBusConnectionString = new ServiceBusConnectionStringBuilder
			{
				Endpoint = serviceBusConfiguration.Endpoint,
				SasKeyName = serviceBusConfiguration.SharedAccessKeyName, 
				SasKey = serviceBusConfiguration.SharedAccessKeyToken
			}.GetNamespaceConnectionString();
		}

		public IQueueClient Create(string queueName)
		{

            // TODO 
			 return new QueueClient(_serviceBusConnectionString, queueName);

            //return new 
		}
	}
}