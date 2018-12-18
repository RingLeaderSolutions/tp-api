using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Rest.Azure;
using NUnit.Framework;
using Theta.Platform.Messaging.ServiceBus.Configuration;
using Theta.Platform.Messaging.ServiceBus.Factories;

namespace Theta.Platform.Messaging.ServiceBus.Tests
{
	[TestFixture]
	public class QueueCreationIntegrationTests
	{
		private ServiceBusCommandQueueClient _commandQueueClient;
		private ServiceBusTestConfiguration _configuration;
		private IServiceBusNamespace _serviceBusNamespace;

		[SetUp]
		public async Task Setup()
		{
			_configuration = new ServiceBusTestConfiguration();
			_serviceBusNamespace = await GetNamespace(_configuration);

			_commandQueueClient = new ServiceBusCommandQueueClient(
				new ServiceBusNamespaceFactory(_configuration), 
				new QueueClientFactory(_configuration));
		}

		[Test, Explicit]
		public async Task CreatesQueueIfDoesNotExist()
		{
			var uniqueQueueName = $"test-{Guid.NewGuid()}";

			await _commandQueueClient.CreateQueueIfNotExists(uniqueQueueName);

			var queues = await _serviceBusNamespace.Queues.ListAsync();
			var createdQueue = queues.FirstOrDefault(q => q.Name.Equals(uniqueQueueName, StringComparison.Ordinal));

			Assert.IsNotNull(createdQueue, $"Unable to verify that [QueueName={uniqueQueueName}] was successfully created.");

			// Cleanup
			await _serviceBusNamespace.Queues.DeleteByNameAsync(uniqueQueueName);

			queues = await _serviceBusNamespace.Queues.ListAsync();
			createdQueue = queues.FirstOrDefault(q => q.Name.Equals(uniqueQueueName, StringComparison.Ordinal));

			Assert.IsNull(createdQueue, $"Queue was created successfully, but deletion failed. Manual deletion required: [QueueName={uniqueQueueName}]");
		}

		[Test, Explicit]
		public void BubblesExceptionIfQueueNameInvalid()
		{
			var invalidQueueName = "$";

			Assert.ThrowsAsync(typeof(CloudException), async () => await _commandQueueClient.CreateQueueIfNotExists(invalidQueueName));
		}

		private async Task<IServiceBusNamespace> GetNamespace(IServiceBusManagementConfiguration config)
		{
			var credentials = SdkContext.AzureCredentialsFactory
				.FromServicePrincipal(
					config.ClientId,
					config.ClientSecret,
					config.TenantId,
					AzureEnvironment.AzureGlobalCloud);

			var manager = ServiceBusManager.Authenticate(
				credentials,
				config.SubscriptionId);

			return await manager.Namespaces
				.GetByResourceGroupAsync(
					config.ResourceGroup,
					config.ResourceName);
		}
	}
}