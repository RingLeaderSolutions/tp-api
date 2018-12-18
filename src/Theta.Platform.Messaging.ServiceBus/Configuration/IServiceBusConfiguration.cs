namespace Theta.Platform.Messaging.ServiceBus.Configuration
{
	public interface IServiceBusConfiguration
	{
		string Endpoint { get; }

		string SharedAccessKeyName { get; }

		string SharedAccessKeyToken { get; }
	}
}