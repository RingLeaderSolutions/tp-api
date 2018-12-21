using Theta.Platform.Messaging.ServiceBus.Configuration;

namespace Theta.Platform.RFQ.Management.Service.Configuration
{
    public class ServiceBusConfiguration : IServiceBusConfiguration, IServiceBusManagementConfiguration
    {
        public string Endpoint { get; set; }
        public string SharedAccessKeyName { get; set; }
        public string SharedAccessKeyToken { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ResourceGroup { get; set; }
        public string ResourceName { get; set; }
    }
}
