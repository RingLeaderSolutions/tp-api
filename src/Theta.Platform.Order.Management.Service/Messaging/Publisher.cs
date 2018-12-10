using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theta.Platform.Order.Management.Service.Configuration;

namespace Theta.Platform.Order.Management.Service.Messaging
{
    public class Publisher : IPublisher
    {
        private readonly ITopicClient _topicClient;
        private readonly IPubSubConfiguration _pubSubConfiguration;
        

        public Publisher(IPubSubConfiguration pubSubConfiguration, string topic)
        {
            _pubSubConfiguration = pubSubConfiguration;
            _topicClient = new TopicClient(pubSubConfiguration.ConnectionString, topic);
        }

        public async Task PublishAsync<T>(T obj)
        {
            var messageText = JsonConvert.SerializeObject(obj);

            var message = new Message(Encoding.UTF8.GetBytes(messageText));

            await _topicClient.SendAsync(message);
        }

        public async Task<long> PublishWithDelay<T>(T obj, DateTimeOffset offestEnqueueTime)
        {
            var messageText = JsonConvert.SerializeObject(obj);

            var message = new Message(Encoding.UTF8.GetBytes(messageText));

            return await _topicClient.ScheduleMessageAsync(message, offestEnqueueTime);
        }
    }
}
