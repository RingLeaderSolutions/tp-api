//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Theta.Platform.Order.Management.Service.Configuration;
//using Theta.Platform.Order.Management.Service.Domain.Commands;
//using Theta.Platform.Order.Management.Service.Framework;

//namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
//{
//    public class CreateOrderSubscriber : Subscriber<CreateOrderCommand>, ISubscriber<CreateOrderCommand>
//    {
//        protected override string SubscriptionName => "create-order_order-management-service";

//        protected override Subscription Subscription => this.PubSubConfiguration.Subscriptions.FirstOrDefault(x => x.SubscriptionName == SubscriptionName);

//        public CreateOrderSubscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IAggregateRepository orderRepository)
//            : base(pubsubResourceManager, pubSubConfiguration, orderRepository)
//        {
            
//        }

//        public override async Task ProcessMessageAsync(CreateOrderCommand createOrderCommand, IAggregateRepository orderRepository)
//        {
//            var order = await orderRepository.GetAsync<Domain.Order>(createOrderCommand.OrderId);

//            if (!IsAggregateNull(order))
//            {
//                order.RaiseInvalidRequestEvent("Create", "Order already exists");
//                await orderRepository.Save(order);
//                return;
//            }

//            await orderRepository.Save(new Domain.Order(
//                createOrderCommand.DeskId, createOrderCommand.OrderId,
//                createOrderCommand.ParentOrderId,
//                createOrderCommand.InstrumentId,
//                createOrderCommand.OwnerId,
//                createOrderCommand.Quantity,
//                createOrderCommand.Type,
//                createOrderCommand.LimitPrice,
//                createOrderCommand.CurrencyCode,
//                createOrderCommand.MarkupUnit,
//                createOrderCommand.MarkupValue));
//        }
//    }
//}
