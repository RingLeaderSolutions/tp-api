using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Order.Management.Service.Messaging.Subscribers;

namespace Theta.Platform.Order.Management.Service.Messaging
{
    public sealed class OrderSubscriber : IHostedService
    {
        private const string QueueName = "order-service";

        private readonly ICommandQueueClient _commandQueueClient;
        private readonly IAggregateReader<Domain.Aggregate.Order> _aggregateReader;
        private readonly Dictionary<string, List<ISubscriber<ICommand, IEvent>>> _commandToSubscriberDictionary;

        private IDisposable _commandSubscription;

        public OrderSubscriber(
	        ICommandQueueClient commandQueueClient,
			IAggregateReader<Domain.Aggregate.Order> aggregateReader,
			IEnumerable<ISubscriber<ICommand, IEvent>> subscribers)
        {
            _commandQueueClient = commandQueueClient;
            _aggregateReader = aggregateReader;

            _commandToSubscriberDictionary = subscribers
				.GroupBy(s => s.CommandType)
				.ToDictionary(s => s.Key.Name, s => s.ToList());
        }

        // TODO: An exception here is indicative of a fatal messaging communication exception, meaning the application should probably crash
		public async Task StartAsync(CancellationToken cancellationToken)
        {
	        await _aggregateReader.StartAsync();

	        await _commandQueueClient.CreateQueueIfNotExists(QueueName);

	        _commandSubscription = _commandQueueClient.GetCommandQueueStream(QueueName)
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
	        _commandSubscription.Dispose();
	        _aggregateReader.Dispose();

            return Task.CompletedTask;
        }
    }
}
