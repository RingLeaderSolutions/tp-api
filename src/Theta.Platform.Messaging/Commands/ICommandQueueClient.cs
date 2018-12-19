using System;
using System.Threading.Tasks;

namespace Theta.Platform.Messaging.Commands
{
	public interface ICommandQueueClient
	{
		Task CreateQueueIfNotExists(string queueName);
		IObservable<IActionableMessage<ICommand>> Subscribe(string queueName);
		Task Send(string queueName, ICommand command);
	}
}
