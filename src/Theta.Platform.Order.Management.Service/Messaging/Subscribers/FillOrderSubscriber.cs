using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public class FillOrderSubscriber : Subscriber<FillOrderCommand>, ISubscriber<FillOrderCommand>
    {
        protected override string SubscriptionName => "fill-order_order-management-service";

        protected override Subscription Subscription => this.PubSubConfiguration.Subscriptions.FirstOrDefault(x => x.SubscriptionName == SubscriptionName);

        public FillOrderSubscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IAggregateRepository orderRepository)
            : base(pubsubResourceManager, pubSubConfiguration, orderRepository)
        {

        }

        public override async Task ProcessMessageAsync(FillOrderCommand fillOrderCommand, IAggregateRepository orderRepository)
        {
            Console.WriteLine("Recieved Message");

            var order = await orderRepository.GetAsync<Domain.Order>(fillOrderCommand.OrderId);

            if (IsAggregateNull(order))
            {
                // IsNull Handle, Log, etc
                return;
            }

            if (order.Status != OrderStatus.Working && order.Status != OrderStatus.PartiallyFilled)
            {
                order.RaiseInvalidRequestEvent("Fill", "Order not in Working Status when Fill requested");
            }
            else if (order.OustandingQuantity < fillOrderCommand.Quantity)
            {
                order.RaiseInvalidRequestEvent("Fill", $"FillOrderCommand called with Quantity of {fillOrderCommand.Quantity} but Order has OustandingQuantity of {order.OustandingQuantity}");
            }
            else
            {
                order.Fill(fillOrderCommand.OrderId, fillOrderCommand.RFQId, fillOrderCommand.Price, fillOrderCommand.Quantity);
            }

            await orderRepository.Save(order);
        }
    }
}
