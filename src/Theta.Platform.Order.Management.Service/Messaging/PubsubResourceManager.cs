using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;

namespace Theta.Platform.Order.Management.Service.Messaging
{
    public class PubsubResourceManager : IPubsubResourceManager
    {
        private readonly IServiceBusNamespace _serviceBusNamespace;
        private readonly IPubSubConfiguration _pubSubConfiguration;

        public PubsubResourceManager(IPubSubConfiguration pubSubConfiguration)
        {
            _pubSubConfiguration = pubSubConfiguration;

            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                _pubSubConfiguration.ClientId, _pubSubConfiguration.ClientSecret, _pubSubConfiguration.TenantId, AzureEnvironment.AzureGlobalCloud);

            var serviceBusManager = ServiceBusManager.Authenticate(credentials, _pubSubConfiguration.SubscriptionId);

            _serviceBusNamespace = serviceBusManager.Namespaces.GetByResourceGroup(_pubSubConfiguration.ResourceGroup, _pubSubConfiguration.NamespaceName);
        }

        public async Task<ITopic> EnsureTopicExists(string topicName)
        {
            ITopic topic = await EnsureTopicAsync(topicName);

            return topic;
        }

        public async Task<ITopic> EnsureTopicSubscriptionExists(string topicName, string subscriptionName)
        {
            ITopic topic = await EnsureTopicAsync(topicName);

            var subscriptions = await topic.Subscriptions.ListAsync();

            var subscription = subscriptions?.FirstOrDefault(sub => sub.Name == subscriptionName);

            if (subscription == null)
            {
                await topic.Subscriptions.Define(subscriptionName)
                    .WithDefaultMessageTTL(TimeSpan.FromDays(14))
                    .WithMessageLockDurationInSeconds(30)
                    .WithMessageMovedToDeadLetterSubscriptionOnFilterEvaluationException()
                    .CreateAsync();
            }

            return topic;
        }

        private async Task<ITopic> EnsureTopicAsync(string topicName)
        {
            var topics = await _serviceBusNamespace.Topics.ListAsync();

            var topic = topics?.FirstOrDefault(tp => tp.Name == topicName);

            if (topic == null)
            {
                topic = await _serviceBusNamespace.Topics.Define(topicName)
                    .WithPartitioning()
                    .WithSizeInMB(2048)
                    .CreateAsync();
            }

            return topic;
        }
    }
}
