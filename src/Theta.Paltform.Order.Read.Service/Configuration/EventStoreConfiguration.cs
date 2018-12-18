using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Messaging.EventStore.Configuration;

namespace Theta.Paltform.Order.Read.Service.Configuration
{
    public class EventStoreConfiguration : IEventStoreConfiguration
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Endpoint { get; set; }
    }
}
