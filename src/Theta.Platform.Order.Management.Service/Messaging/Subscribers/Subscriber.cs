using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Framework;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public abstract class Subscriber<T>
    {
        private readonly IPubsubResourceManager _pubsubResourceManager;
        private readonly IAggregateRepository _orderRepository;
        private readonly ISubscriptionClient _subscriptionClient;
        
        protected abstract string SubscriptionName { get; }
        protected abstract Subscription Subscription { get; }

        public IPubSubConfiguration PubSubConfiguration { get; }

        protected Subscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IAggregateRepository orderRepository)
        {
            PubSubConfiguration = pubSubConfiguration;

            _pubsubResourceManager = pubsubResourceManager;
            _orderRepository = orderRepository;

            // Ensure the Command topic and subscription exist
            _pubsubResourceManager.EnsureTopicSubscriptionExists(Subscription.TopicName, Subscription.SubscriptionName);

            _subscriptionClient = new SubscriptionClient(PubSubConfiguration.ConnectionString, Subscription.TopicName, Subscription.SubscriptionName);
        }

        public void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 10,
                AutoComplete = false
            };
            
            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Processing message: {message.MessageId}");

            var messageText = Encoding.UTF8.GetString(message.Body);

            var entity = JsonConvert.DeserializeObject<T>(messageText);

            await this.ProcessMessageAsync(entity, _orderRepository);

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        protected bool IsAggregateNull<V>(V aggregate) where V : IAggregateRoot
        {
            return aggregate == null || aggregate.Version == -1;
        }

        public abstract Task ProcessMessageAsync(T obj, IAggregateRepository orderRepository);

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            // Log - Handle - Retry
            return Task.CompletedTask;
        }
    }
}