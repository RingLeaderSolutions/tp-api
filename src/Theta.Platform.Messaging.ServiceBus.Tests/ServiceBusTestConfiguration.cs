using Theta.Platform.Messaging.ServiceBus.Configuration;

namespace Theta.Platform.Messaging.ServiceBus.Tests
{
	public sealed class ServiceBusTestConfiguration : IServiceBusConfiguration, IServiceBusManagementConfiguration
	{
		public string Endpoint => "sb://thetadevelopment.servicebus.windows.net/";
		public string SharedAccessKeyName => "RootManageSharedAccessKey";
		public string SharedAccessKeyToken => "8AWjcMELbwP0KLBIHhO7sMnbFnL32fgOYYZTF+kZsOU=";
		public string ClientId => "b9c49b19-c12d-4ef9-9ede-ed7b08bb31e8";
		public string ClientSecret => "w2MUEVCNJpmwmO";
		public string TenantId => "72efa66b-c4aa-41a8-8540-3c0b60139858";
		public string SubscriptionId => "d3f52271-c4a3-41bc-995d-ee910659772f";
		public string ResourceGroup => "ThetaDevRGVM";
		public string ResourceName => "thetadevelopment";
	}
}