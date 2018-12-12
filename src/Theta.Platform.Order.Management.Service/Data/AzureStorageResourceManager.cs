using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;

namespace Theta.Platform.Order.Management.Service.Data
{
    public class AzureStorageResourceManager : IAzureStorageResourceManager
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _ordersTable;
        private readonly IDatastorageConfiguration _configuration;

        public AzureStorageResourceManager(IDatastorageConfiguration configuration)
        {
            _configuration = configuration;
            _storageAccount = CloudStorageAccount.Parse(_configuration.ConnectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();

            
        }

        public async Task CreateOrdersTableAsync()
        {
            var ordersTable = _tableClient.GetTableReference("orders");

            await ordersTable.CreateIfNotExistsAsync();
        }
    }
}
