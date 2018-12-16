using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class RRegisterSupplementaryEvidenceCommandSubscriber : Subscriber<RegisterSupplementaryEvidenceCommand>, ISubscriber<RegisterSupplementaryEvidenceCommand>
    {
        protected override string SubscriptionName => "rse-order_order-management-service";

        protected override Subscription Subscription => this.PubSubConfiguration.Subscriptions.FirstOrDefault(x => x.SubscriptionName == SubscriptionName);

        public RRegisterSupplementaryEvidenceCommandSubscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IAggregateRepository orderRepository)
            : base(pubsubResourceManager, pubSubConfiguration, orderRepository)
        {
            
        }

        public override async Task ProcessMessageAsync(RegisterSupplementaryEvidenceCommand completeOrderCommand, IAggregateRepository orderRepository)
        {
            Console.WriteLine("Recieved Message");

            var order = await orderRepository.GetAsync<Domain.Order>(completeOrderCommand.OrderId);

            if (order == null)
            {
                // Handle
            }

            if (order != null)
            {
                order.RaiseInvalidRequestEvent("RegisterSupplementaryEvidence", "Order not in Complete Status when RegisterSupplementaryEvidence requested");
                return;
            }

            order.RecordSupplementaryEvidence(completeOrderCommand.SupplementaryEvidence);

            await orderRepository.Save(order);
        }
    }
}
