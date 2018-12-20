using System;
using System.Threading.Tasks;

namespace Theta.Platform.Messaging.Events
{
	public interface IEventPersistenceClient
	{
		Task<IEvent[]> Retrieve(Type eventType);
		Task Save(string streamName, int expectedVersion, IEvent domainEvents);
	}
}