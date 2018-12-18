using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Messaging.ServiceBus
{
	public sealed class ServiceBusActionableMessage<TCommand> : IActionableMessage<TCommand>
	{
		private readonly IQueueClient _queueClient;
		private readonly Message _message;

		public ServiceBusActionableMessage(IQueueClient queueClient, Message message, TCommand receivedCommand)
		{
			_queueClient = queueClient;
			_message = message;
			ReceivedCommand = receivedCommand;
		}

		public TCommand ReceivedCommand { get; }

		public async Task Complete()
		{
			await _queueClient.CompleteAsync(this._message.SystemProperties.LockToken);
		}

		public async Task Abandon()
		{
			await _queueClient.AbandonAsync(this._message.SystemProperties.LockToken);
		}

		public async Task Reject(string reason, string additionalDescription)
		{
			await _queueClient.DeadLetterAsync(this._message.SystemProperties.LockToken, reason, additionalDescription);
		}
	}
}