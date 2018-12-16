using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class PutDownOrderSubscriber : Subscriber<PutDownOrderCommand>, ISubscriber<PutDownOrderCommand>
    {
        protected override string SubscriptionName => "putdown-order_order-management-service";

        protected override Subscription Subscription => this.PubSubConfiguration.Subscriptions.FirstOrDefault(x => x.SubscriptionName == SubscriptionName);

        public PutDownOrderSubscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IAggregateRepository orderRepository)
            : base(pubsubResourceManager, pubSubConfiguration, orderRepository)
        {
            
        }

        public override async Task ProcessMessageAsync(PutDownOrderCommand completeOrderCommand, IAggregateRepository orderRepository)
        {
            Console.WriteLine("Recieved Message");

            var order = await orderRepository.GetAsync<Domain.Order>(completeOrderCommand.OrderId);

            if (IsAggregateNull(order))
            {
                // IsNull Handle, Log, etc
            }

            if (order.Status != OrderStatus.Working)
            {
                order.RaiseInvalidRequestEvent("PutDown", "Order not in Working Status when PutDown requested");
                await orderRepository.Save(order);
                return;
            }

            order.PutDown();

            await orderRepository.Save(order);
        }
    }
}
