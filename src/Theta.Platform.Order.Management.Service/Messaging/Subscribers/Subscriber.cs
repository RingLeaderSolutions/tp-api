using System.Threading.Tasks;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public abstract class Subscriber<T> : ISubscriber<T> where T : ICommand
    {
	    public abstract Task Handle(T command);
    }
}