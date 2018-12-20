using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.ServiceBus.Factories;

namespace Theta.Platform.Messaging.ServiceBus
{
	public sealed class ServiceBusCommandQueueClient : ICommandQueueClient
	{
		private readonly Dictionary<string, Type> _commandTypeDictionary;
		private readonly IServiceBusNamespaceFactory _serviceBusNamespaceFactory;
		private readonly IQueueClientFactory _queueClientFactory;

		public ServiceBusCommandQueueClient(
			Dictionary<string, Type> commandTypeDictionary,
			IServiceBusNamespaceFactory serviceBusNamespaceFactory,
			IQueueClientFactory queueClientFactory)
		{
			_commandTypeDictionary = commandTypeDictionary;
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

			await ns.Queues
				.Define(queueName)
				.WithSizeInMB(1024)
				.WithMessageMovedToDeadLetterQueueOnMaxDeliveryCount(5)
				.WithDefaultMessageTTL(TimeSpan.FromDays(14))
				.CreateAsync();
		}

		public IObservable<IActionableMessage<ICommand>> Subscribe(string queueName)
		{
			var qc = _queueClientFactory.Create(queueName);

			return Observable.Create((IObserver<IActionableMessage<ICommand>> obs) =>
			{
				var exceptionHandler = new Func<ExceptionReceivedEventArgs, Task>(
					exceptionArgs =>
					{
						// TODO: Logging here
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
							var jsonObject = JObject.Parse(jsonBody);
							if (!jsonObject.TryGetValue("type", StringComparison.OrdinalIgnoreCase, out JToken typeToken))
							{
								throw new JsonException("Unable to find the required field [type] on the received command JSON");
							}

							var type = typeToken.Value<string>();

							// If we receive a command message that represents a type that we do not recognize,
							// requeue with the intention of it being consumed by another (maybe newer) client
							if (!_commandTypeDictionary.TryGetValue(type, out Type commandType))
                            {
								// TODO: Logging here
								// TODO: Potentially properties of message to ensure that it is not received by the same consumer again and again? 
								// TODO: Automatic dead-lettering is configured in CreateQueueIfNotExists() above, but we may want to implement our own logic using message.SystemProperties.DeliveryCount & other props
								await qc.AbandonAsync(message.SystemProperties.LockToken);
								return;
                            }

                            var typedCommand = JsonConvert.DeserializeObject(jsonBody, commandType);
							var messageWrapper = new ServiceBusActionableMessage<ICommand>(qc, message, (ICommand)typedCommand);

							obs.OnNext(messageWrapper);
						}
						catch (Exception ex)
						{
							// TODO: Logging here
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