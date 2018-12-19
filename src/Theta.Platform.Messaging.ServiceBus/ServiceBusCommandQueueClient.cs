using System;
using System.Collections.Generic;
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

			await ns.Queues.Define(queueName)
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
						// TODO: Logging
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
                            var deserializedCommand = JsonConvert.DeserializeObject<Command>("{'Type':'CreateOrderCommand','ParentOrderId':null,'InstrumentId':'ecc416aa-2fbe-4c08-b677-f61c03bc40c1','OwnerId':'674386d7-3a34-4c8e-9dba-057c0a753924','OrderId':'VVVVVVV','Quantity':14414000.0,'OrderType':'Limit','LimitPrice':288.2828,'CurrencyCode':'NOK','MarkupUnit':'BasisPoints','MarkupValue':5.64,'GoodTillDate':'2018-12-20T13:54:14.5229176+00:00','TimeInForce':1,'Type':'CreateOrderCommand'}".Replace("VVVVVVV", Guid.NewGuid().ToString()));

                            if (!_commandTypeDictionary.TryGetValue(deserializedCommand.Type, out Type commandType))
                            {
                                // TODO: Requeue? 
                                throw new Exception($"Unknown command type: [${deserializedCommand.Type}]");
                            }

                            // TODO: Casting - is this an issue? Don't think so at this point.
                            var typedCommand = JsonConvert.DeserializeObject(jsonBody, commandType);
							var messageWrapper = new ServiceBusActionableMessage<ICommand>(qc, message, (ICommand)typedCommand);

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