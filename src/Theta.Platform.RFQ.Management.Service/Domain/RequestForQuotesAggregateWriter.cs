using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Events;
using Theta.Platform.RFQ.Management.Service.Domain.Events;

namespace Theta.Platform.RFQ.Management.Service.Domain
{
    public class RequestForQuotesAggregateWriter : AggregateWriter<RequestForQuotes>
    {
        public RequestForQuotesAggregateWriter(
            IEventPersistenceClient eventPersistenceClient,
            IEventStreamingClient eventStreamingClient) :
            base(eventPersistenceClient, eventStreamingClient)
        {
        }

        protected override Dictionary<string, Type> SubscribedEventTypes { get; } = new List<KeyValuePair<string, Type>>
        {
            CreateEventNameToTypeMapping(typeof(RFQRaisedEvent)),
            CreateEventNameToTypeMapping(typeof(RFQCancelledEvent)),
            CreateEventNameToTypeMapping(typeof(RFQQuoteReceivedEvent)),
            CreateEventNameToTypeMapping(typeof(RFQQuoteRetractedEvent)),
        }.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        private static KeyValuePair<string, Type> CreateEventNameToTypeMapping(Type type)
        {
            return new KeyValuePair<string, Type>(type.Name, type);
        }
    }
}