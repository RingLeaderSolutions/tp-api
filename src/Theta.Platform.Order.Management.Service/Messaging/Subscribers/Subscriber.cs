﻿using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Data;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public abstract class Subscriber<T, V>
    {
        private readonly IPubsubResourceManager _pubsubResourceManager;
        private readonly IOrderRepository _orderRepository;
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly IPublisher _eventPublisher;

        protected abstract string SubscriptionName { get; }
        protected abstract Subscription Subscription { get; }

        public IPubSubConfiguration PubSubConfiguration { get; }

        protected Subscriber(IPubsubResourceManager pubsubResourceManager, IPubSubConfiguration pubSubConfiguration, IOrderRepository orderRepository)
        {
            PubSubConfiguration = pubSubConfiguration;

            _pubsubResourceManager = pubsubResourceManager;
            _orderRepository = orderRepository;

            // Ensure the event topic exists
            _pubsubResourceManager.EnsureTopicExists(Subscription.EventTopicName);

            _eventPublisher = new Publisher(PubSubConfiguration, Subscription.EventTopicName);

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

            var evt = await this.ProcessAsync(entity, message.MessageId, _orderRepository);

            await _eventPublisher.PublishAsync(evt);

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        public abstract Task<V> ProcessAsync(T obj, string messageId, IOrderRepository orderRepository);

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            // Log - Handle - Retry
            return Task.CompletedTask;
        }
    }
}