using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.ServiceBus.Factories;

namespace Theta.Platform.Messaging.ServiceBus
{
	public sealed class ServiceBusCommandQueueClient : ICommandQueueClient
	{
		private readonly IServiceBusNamespaceFactory _serviceBusNamespaceFactory;
		private readonly IQueueClientFactory _queueClientFactory;

		public ServiceBusCommandQueueClient(
			IServiceBusNamespaceFactory serviceBusNamespaceFactory,
			IQueueClientFactory queueClientFactory)
		{
			_serviceBusNamespaceFactory = serviceBusNamespaceFactory;
			_queueClientFactory = queueClientFactory;
		}

		public async Task CreateQueueIfNotExists(string queueName)
		{
			var ns = await _serviceBusNamespaceFactory.Create();

			var queues = await ns.Queues.ListAsync();
			var queue = queues.FirstOrDefault(q => q.Name.Equals(queueName, StringComparison.Ordinal));

			if (queue != null)
			{
				return;
			}

			await ns.Queues.Define(queueName)
				.WithSizeInMB(1024)
				.WithMessageMovedToDeadLetterQueueOnMaxDeliveryCount(5)
				.WithDefaultMessageTTL(TimeSpan.FromDays(14))
				.CreateAsync();
		}

		public IObservable<IActionableMessage<TCommand>> Subscribe<TCommand>(string queueName)
		{
			var qc = _queueClientFactory.Create(queueName);

			return Observable.Create((IObserver<IActionableMessage<TCommand>> obs) =>
			{
				var exceptionHandler = new Func<ExceptionReceivedEventArgs, Task>(
					exceptionArgs =>
					{
						obs.OnError(exceptionArgs.Exception);
						return Task.CompletedTask;
					});

				var messageHandlerOptions = new MessageHandlerOptions(exceptionHandler)
				{
					MaxConcurrentCalls = 1,
					AutoComplete = false
				};

				qc.RegisterMessageHandler(
					async (message, cancellationToken) =>
					{
						try
						{
							var jsonBody = Encoding.UTF8.GetString(message.Body);
							var deserializedCommand = JsonConvert.DeserializeObject<TCommand>(jsonBody);
							var messageWrapper = new ServiceBusActionableMessage<TCommand>(qc, message, deserializedCommand);

							obs.OnNext(messageWrapper);
						}
						catch (Exception ex)
						{
							// Note: We purposely do not onError the stream at this point, as deserialization errors 
							// are not a valid reason to kill the entire subscription, and instead should just be dead-lettered
							await qc.DeadLetterAsync(message.SystemProperties.LockToken, "Failed to deserialize message", ex.Message);
						}
					},
					messageHandlerOptions);

				return Disposable.Create(
					async () =>
					{
						await qc.CloseAsync();
					});
			});
		}

		public async Task Send(string queueName, ICommand command)
		{
			var qc = _queueClientFactory.Create(queueName);

			try
			{
				var serializedCommand = JsonConvert.SerializeObject(command);
				var commandBytes = Encoding.UTF8.GetBytes(serializedCommand);
				var message = new Message(commandBytes);

				await qc.SendAsync(message);
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}