using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Data
{
    public interface IAzureStorageResourceManager
    {
        Task CreateOrdersTableAsync();
    }
}