using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace Theta.Platform.Order.Management.Service.Messaging
{
    public interface IPubsubResourceManager
    {
        Task<ITopic> EnsureTopicExists(string topicName);
        Task<ITopic> EnsureTopicSubscriptionExists(string topicName, string subscriptionName);
    }
}