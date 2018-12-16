using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class CompleteOrderSubscriber : Subscriber<CompleteOrderCommand>, ISubscriber<CompleteOrderCommand>
    {
        protected override string SubscriptionName => "complete-order_order-management-service";

        protected override Subscription Subscription => this.PubSubConfiguration.Subscriptions.FirstOrDefault(x => x.SubscriptionName == SubscriptionName);

        public CompleteOrderSubscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IAggregateRepository orderRepository)
            : base(pubsubResourceManager, pubSubConfiguration, orderRepository)
        {

        }

        public override async Task ProcessMessageAsync(CompleteOrderCommand completeOrderCommand, IAggregateRepository orderRepository)
        {
            Console.WriteLine("Recieved Message");

            var order = await orderRepository.GetAsync<Domain.Order>(completeOrderCommand.OrderId);

            if (IsAggregateNull(order))
            {
                // IsNull Handle, Log, etc
            }

            if (order.Status != OrderStatus.Filled)
            {
                order.RaiseInvalidRequestEvent("Complete", "Order not in Filled Status when Complete requested");
                await orderRepository.Save(order);
                return;
            }

            order.Complete();

            await orderRepository.Save(order);
        }
    }
}
