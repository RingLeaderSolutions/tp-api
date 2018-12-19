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
    public sealed class OrderSubscriber : IHostedService
    {
        private const string QueueName = "order-service";

        private readonly ICommandQueueClient _commandQueueClient;
        private readonly Dictionary<string, List<ISubscriber<ICommand, IEvent>>> _commandToSubscriberDictionary;

        private IDisposable _subscription;

        public OrderSubscriber(
	        ICommandQueueClient commandQueueClient, 
	        IEnumerable<ISubscriber<ICommand, IEvent>> subscribers)
        {
            _commandQueueClient = commandQueueClient;

			_commandToSubscriberDictionary = subscribers
				.GroupBy(s => s.CommandType)
				.ToDictionary(s => s.Key.Name, s => s.ToList());
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
	        await _commandQueueClient.CreateQueueIfNotExists(QueueName);

	        _subscription = _commandQueueClient.Subscribe(QueueName)
		        .Subscribe(message =>
		        {
			        if (_commandToSubscriberDictionary.TryGetValue(message.ReceivedCommand.Type, out var commandSubscribers))
			        {
				        commandSubscribers.ForEach(async sub => await sub.HandleCommand(message));
				        return;
					}

			        // TODO: Logging here
			        // We don't have a valid subscriber for this command, so abandon it and let someone else process it 
			        message.Abandon();
				},
			    ex =>
		        {
					// TODO: Logging here
					// TODO: An onError here is indicative of a fatal messaging communication exception, meaning the application should probably crash
		        });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
	        _subscription.Dispose();

            return Task.CompletedTask;
        }
    }
}
