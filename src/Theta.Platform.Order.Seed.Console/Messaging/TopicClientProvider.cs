using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using Theta.Platform.Order.Seed.Console.Configuration;
using Theta.Platform.Order.Seed.Console.Messaging.Commands;

namespace Theta.Platform.Order.Seed.Console.Messaging
{
    public class TopicClientProvider
    {
        private readonly PubSubConfiguration _pubsubConfiguration;
        private readonly Dictionary<Type, ITopicClient> _topicClients;

        public TopicClientProvider(PubSubConfiguration pubsubConfiguration)
        {
            _pubsubConfiguration = pubsubConfiguration;

            _topicClients = new Dictionary<Type, ITopicClient>
            {
                { typeof(CompleteOrderCommand), new TopicClient(_pubsubConfiguration.Endpoint, _pubsubConfiguration.CompleteOrderEntityPath) },
                { typeof(CreateOrderCommand), new TopicClient(_pubsubConfiguration.Endpoint, _pubsubConfiguration.CreateOrderEntityPath) },
                { typeof(FillOrderCommand), new TopicClient(_pubsubConfiguration.Endpoint, _pubsubConfiguration.FillOrderEntityPath) },
                { typeof(PickupOrderCommand), new TopicClient(_pubsubConfiguration.Endpoint, _pubsubConfiguration.PickUpOrderEntityPath) },
                { typeof(PutDownOrderCommand), new TopicClient(_pubsubConfiguration.Endpoint, _pubsubConfiguration.PutDownOrderEntityPath) },
                { typeof(RejectOrderCommand), new TopicClient(_pubsubConfiguration.Endpoint, _pubsubConfiguration.RejectOrderEntityPath) },
                { typeof(RegisterSupplementaryEvidenceCommand), new TopicClient(_pubsubConfiguration.Endpoint, _pubsubConfiguration.RseOrderEntityPath) }
            };
        }

        public ITopicClient GetTopicClient(Type type)
        {
            return _topicClients[type];
        }
    }
}
