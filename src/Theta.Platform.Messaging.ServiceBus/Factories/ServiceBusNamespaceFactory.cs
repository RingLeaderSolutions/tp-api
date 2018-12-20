using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Theta.Platform.Messaging.ServiceBus.Configuration;

namespace Theta.Platform.Messaging.ServiceBus.Factories
{
	public sealed class ServiceBusNamespaceFactory : IServiceBusNamespaceFactory
	{
		private readonly IServiceBusManagementConfiguration _managementConfiguration;

		public ServiceBusNamespaceFactory(IServiceBusManagementConfiguration managementConfiguration)
		{
			_managementConfiguration = managementConfiguration;
		}

		public async Task<IServiceBusNamespace> Create()
		{
			var credentials = SdkContext.AzureCredentialsFactory
				.FromServicePrincipal(
					_managementConfiguration.ClientId,
					_managementConfiguration.ClientSecret,
					_managementConfiguration.TenantId,
					AzureEnvironment.AzureGlobalCloud);

			var manager = ServiceBusManager.Authenticate(
				credentials,
				_managementConfiguration.SubscriptionId);

			return await manager.Namespaces
				.GetByResourceGroupAsync(
					_managementConfiguration.ResourceGroup,
					_managementConfiguration.ResourceName);
		}
	}
}