using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class PickupOrderSubscriber : Subscriber<PickupOrderCommand>, ISubscriber<PickupOrderCommand>
    {
        protected override string SubscriptionName => "pickup-order_order-management-service";

        protected override Subscription Subscription => this.PubSubConfiguration.Subscriptions.FirstOrDefault(x => x.SubscriptionName == SubscriptionName);

        public PickupOrderSubscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IAggregateRepository orderRepository)
            : base(pubsubResourceManager, pubSubConfiguration, orderRepository)
        {
           var order =  orderRepository.GetAsync<Domain.Order>(new Guid("24a287a2-f359-4a1f-91ed-989983f4990a")).Result;
        }

        public override async Task ProcessMessageAsync(PickupOrderCommand completeOrderCommand, IAggregateRepository orderRepository)
        {
            Console.WriteLine("Recieved Message");

            var order = await orderRepository.GetAsync<Domain.Order>(completeOrderCommand.OrderId);

            if (IsAggregateNull(order))
            {
                // IsNull Handle, Log, etc
            }

            if (order.Status != OrderStatus.Pending)
            {
                order.RaiseInvalidRequestEvent("Pickup", "Order not in Pending Status when Pickup requested");
                await orderRepository.Save(order);
                return;
            }

            order.Pickup(completeOrderCommand.OwnerId);

            await orderRepository.Save(order);
        }
    }
}
