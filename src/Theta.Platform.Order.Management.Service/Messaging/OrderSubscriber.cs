using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Order.Management.Service.Messaging.Subscribers;

namespace Theta.Platform.Order.Management.Service.Messaging
{
    public class OrderSubscriber : IHostedService
    {
        private const string QueueName = "order-service";
        private readonly ICommandQueueClient _commandQueueClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, List<ISubscriber<ICommand, IEvent>>> _subscribers;

        public OrderSubscriber(ICommandQueueClient commandQueueClient, IServiceProvider serviceProvider)
        {
            _commandQueueClient = commandQueueClient;
            _serviceProvider = serviceProvider;

            _subscribers = new Dictionary<string, List<ISubscriber<ICommand, IEvent>>>
            {
                { typeof(CompleteOrderCommand).Name, GetSubscribers<CompleteOrderCommand, OrderCompletedEvent>() },
                { typeof(CreateOrderCommand).Name, GetSubscribers<CreateOrderCommand, OrderCreatedEvent>() },
                { typeof(FillOrderCommand).Name, GetSubscribers<FillOrderCommand, OrderFilledEvent>() },
                { typeof(PickupOrderCommand).Name, GetSubscribers<PickupOrderCommand, OrderPickedUpEvent>() },
                { typeof(PutDownOrderCommand).Name, GetSubscribers<PutDownOrderCommand, OrderPutDownEvent>() },
                { typeof(RegisterSupplementaryEvidenceCommand).Name, GetSubscribers<RegisterSupplementaryEvidenceCommand, SupplementaryEvidenceReceivedEvent>() },
                { typeof(RejectOrderCommand).Name, GetSubscribers<RejectOrderCommand, OrderRejectedEvent>() }
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Subscribe();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            // Cleanup
        }

        public async Task Subscribe()
        {
            await _commandQueueClient.CreateQueueIfNotExists(QueueName);

            _commandQueueClient.Subscribe(QueueName)
                 .Subscribe(command =>
                 {
                     if (_subscribers.TryGetValue(command.ReceivedCommand.Type, out List<ISubscriber<ICommand, IEvent>> commandSubscribers))
                     {
                         commandSubscribers.ForEach(sub => sub.HandleCommand(command));
                     }
                 });
        }

        private List<ISubscriber<ICommand, IEvent>> GetSubscribers<T, V>() where T : ICommand where V : IEvent
        {
            return _serviceProvider.GetServices<ISubscriber<T, V>>()
                .Cast<ISubscriber<ICommand, IEvent>>().ToList();
        }
    }
}
