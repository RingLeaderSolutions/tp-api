using EventStore.ClientAPI;

namespace Theta.Platform.Messaging.EventStore.Factories
{
	public interface IEventStoreConnectionFactory
	{
		IEventStoreConnection Create();
	}
}