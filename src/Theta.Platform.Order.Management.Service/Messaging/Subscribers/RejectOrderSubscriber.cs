using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class RejectOrderSubscriber : Subscriber<RejectOrderCommand>, ISubscriber<RejectOrderCommand>
    {
        protected override string SubscriptionName => "reject-order_order-management-service";

        protected override Subscription Subscription => this.PubSubConfiguration.Subscriptions.FirstOrDefault(x => x.SubscriptionName == SubscriptionName);

        public RejectOrderSubscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IAggregateRepository orderRepository)
            : base(pubsubResourceManager, pubSubConfiguration, orderRepository)
        {
            
        }

        public override async Task ProcessMessageAsync(RejectOrderCommand completeOrderCommand, IAggregateRepository orderRepository)
        {
            Console.WriteLine("Recieved Message");

            var order = await orderRepository.GetAsync<Domain.Order>(completeOrderCommand.OrderId);

            if (order == null)
            {
                // Handle
            }

            order.Reject(completeOrderCommand.Reason);

            await orderRepository.Save(order);
        }
    }
}
