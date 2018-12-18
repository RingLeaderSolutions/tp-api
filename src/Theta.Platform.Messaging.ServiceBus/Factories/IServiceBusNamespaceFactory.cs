using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace Theta.Platform.Messaging.ServiceBus.Factories
{
	public interface IServiceBusNamespaceFactory
	{
		Task<IServiceBusNamespace> Create();
	}
}