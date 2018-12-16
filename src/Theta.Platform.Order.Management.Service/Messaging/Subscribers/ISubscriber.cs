using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public interface ISubscriber<T>
    {
        void RegisterOnMessageHandlerAndReceiveMessages();

        Task ProcessMessageAsync(T obj, IAggregateRepository orderRepository);
    }
}
