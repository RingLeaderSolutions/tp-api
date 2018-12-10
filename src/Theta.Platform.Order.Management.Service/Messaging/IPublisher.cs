using System;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Messaging
{
    public interface IPublisher
    {
        Task PublishAsync<T>(T obj);

        Task<long> PublishWithDelay<T>(T obj, DateTimeOffset offestEnqueueTime);
    }
}