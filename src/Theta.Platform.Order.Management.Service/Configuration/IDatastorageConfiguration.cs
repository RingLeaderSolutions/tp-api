namespace Theta.Platform.Order.Management.Service.Configuration
{
    public interface IDatastorageConfiguration
    {
        string AccountKey { get; set; }
        string AccountName { get; set; }
        string ConnectionString { get; }
        string DefaultEndpointsProtocol { get; set; }
        string TableEndpoint { get; set; }
    }
}