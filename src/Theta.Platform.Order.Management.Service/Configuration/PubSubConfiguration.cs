using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Configuration
{
    public class PubSubConfiguration : IPubSubConfiguration
    {
        public string ResourceGroup { get; set; }

        public string NamespaceName { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string SubscriptionId { get; set; }

        public string TenantId { get; set; }

        public string Endpoint { get; set; }

        public string SharedAccessKeyName { get; set; }

        public string SharedAccessKey { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"Endpoint={Endpoint};SharedAccessKeyName={SharedAccessKeyName};SharedAccessKey={SharedAccessKey}";
            }
        }

        public Subscription[] Subscriptions { get; set; }
    }

    public class Subscription
    {
        public string TopicName { get; set; }


        public string SubscriptionName { get; set; }


        public string EventTopicName { get; set; }
    }
}
