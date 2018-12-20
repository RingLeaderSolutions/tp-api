namespace Theta.Platform.Messaging.ServiceBus.Configuration
{
	public interface IServiceBusManagementConfiguration
	{
		string ClientId { get; }

		string ClientSecret { get; }

		string TenantId { get; }

		string SubscriptionId { get; }

		string ResourceGroup { get; }

		string ResourceName { get; }
	}
}