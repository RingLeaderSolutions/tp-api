using System;
using Theta.Platform.Messaging.EventStore.Configuration;

namespace Theta.Platform.Order.Read.Service.Configuration
{
    public class EventStoreConfiguration : IEventStoreConfiguration
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public Uri Endpoint { get; set; }
    }
}
