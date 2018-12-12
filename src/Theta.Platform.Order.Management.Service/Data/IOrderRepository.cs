using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Data
{
    public interface IOrderRepository
    {
        Task CreateOrder(Order order, string messageId);

        Task<List<Order>> GetOrder(Guid orderId);

        Task<List<Order>> GetOrdersForOwner(Guid ownerId);

        Task<List<Order>> GetOrdersForEntity(Guid entityId);
    }
}