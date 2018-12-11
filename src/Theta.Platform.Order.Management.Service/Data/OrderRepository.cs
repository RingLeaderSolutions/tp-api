using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Data.Entities;

namespace Theta.Platform.Order.Management.Service.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _ordersTable;
        private readonly IDatastorageConfiguration _configuration;

        public OrderRepository(IDatastorageConfiguration configuration)
        {
            _configuration = configuration;
            _storageAccount = CloudStorageAccount.Parse(_configuration.ConnectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();

            _ordersTable = _tableClient.GetTableReference("orders");
        }

        public async Task CreateOrder(Order order, string messageId)
        {
            OrderEntity orderEntity = order.ToOrderEntity(messageId);

            TableOperation insertOperation = TableOperation.Insert(orderEntity);

            var result = await _ordersTable.ExecuteAsync(insertOperation);
        }

        public async Task<List<Order>> GetOrder(Guid orderId)
        {
            var results = await _ordersTable.ExecuteQueryAsync(Query("PartitionKey", orderId));

            return results?.Select(order => order.ToOrder()).ToList();
        }

        public async Task<List<Order>> GetOrdersForOwner(Guid ownerId)
        {
            var results = await _ordersTable.ExecuteQueryAsync(Query("OwnerId", ownerId));

            return results?.Select(order => order.ToOrder()).ToList();
        }

        public async Task<List<Order>> GetOrdersForEntity(Guid entityId)
        {
            var results = await _ordersTable.ExecuteQueryAsync(Query("EntityId", entityId));

            return results?.Select(order => order.ToOrder()).ToList();
        }

        private static TableQuery<OrderEntity> Query(string propertyName, Guid objectId)
        {
            return new TableQuery<OrderEntity>()
                             .Where(TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, objectId.ToString()));
        }
    }
}
