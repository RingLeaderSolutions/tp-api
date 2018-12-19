using System.Threading.Tasks;
using Theta.Platform.Messaging.Commands;

namespace Theta.Platform.Order.Management.Service.Messaging.Subscribers
{
    public interface ISubscriber<in T> where T : ICommand
    {
	    Task Handle(T command);
    }
}
