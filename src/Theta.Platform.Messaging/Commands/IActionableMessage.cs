using System.Threading.Tasks;

namespace Theta.Platform.Messaging.Commands
{
	public interface IActionableMessage<out TCommand>
	{
		TCommand ReceivedCommand { get; }
		Task Complete();
		Task Abandon();
		Task Reject(string reason, string additionalDescription);
	}
}