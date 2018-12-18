using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Events;

namespace Theta.Paltform.Order.Read.Service.Data
{
    public interface IOrderReader
    {
        Task StartAsync();

        Domain.Order[] Get();

        Domain.Order GetById(Guid id);
    }

    public class OrderReader : AggregateReader<Domain.Order>, IOrderReader
    {
        public OrderReader(IEventPersistenceClient eventPersistenceClient, IEventStreamingClient eventStreamingClient) 
            : base(eventPersistenceClient, eventStreamingClient)
        {
        }

        public override Dictionary<string, Type> GetEventTypes()
        {
            var collection = new Dictionary<string, Type>();

            AddEventType(collection, typeof(OrderCompletedEvent));
            AddEventType(collection, typeof(OrderCreatedEvent));
            AddEventType(collection, typeof(OrderFilledEvent));
            AddEventType(collection, typeof(OrderPickedUpEvent));
            AddEventType(collection, typeof(OrderPickUpRejectedEvent));
            AddEventType(collection, typeof(OrderPutDownEvent));
            AddEventType(collection, typeof(OrderRejectedEvent));
            AddEventType(collection, typeof(SupplementaryEvidenceReceivedEvent));

            return collection;
        }

        private static void AddEventType(Dictionary<string, Type> collection, Type type)
        {
            collection.Add(type.Name, type);
        }
    }
}
