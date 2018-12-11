namespace Theta.Platform.Order.Management.Service.Configuration
{
    public interface IPubSubConfiguration
    {
        string Endpoint { get; set; }
        string SharedAccessKeyName { get; set; }
        string SharedAccessKey { get; }
        string ConnectionString { get; }
        string ResourceGroup { get; set; }
        string NamespaceName { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string SubscriptionId { get; set; }
        string TenantId { get; set; }
        Subscription[] Subscriptions { get; set; }
    }
}