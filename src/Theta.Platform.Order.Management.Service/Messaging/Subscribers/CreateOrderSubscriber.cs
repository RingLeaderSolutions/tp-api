using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Data;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Messaging.MessageContracts;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class CreateOrderSubscriber : Subscriber<CreateOrderCommand>, ISubscriber<CreateOrderCommand>
    {
        protected override string SubscriptionName => "create-order_order-management-service";

        protected override Subscription Subscription => this.PubSubConfiguration.Subscriptions.FirstOrDefault(x => x.SubscriptionName == SubscriptionName);

        private readonly IPublisher _orderExprationPublisher;

        public CreateOrderSubscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IOrderRepository orderRepository)
            : base(pubsubResourceManager, pubSubConfiguration, orderRepository)
        {
            // Publish an Auto-Expration command?
            _orderExprationPublisher = new Publisher(pubSubConfiguration, "cancel-order");
        }

        public override async Task ProcessMessageAsync(CreateOrderCommand createOrderCommand, string messageId, IOrderRepository orderRepository)
        {
            Console.WriteLine("Recieved Message");

            // replace with OrderProcessManager (kill repo and all ref to Table Storage)
            await orderRepository.CreateOrder(createOrderCommand, messageId);
        }
    }
}
