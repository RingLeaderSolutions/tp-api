using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain
{
	public interface IAggregateWriter<out TAggregate> : IAggregateReader<TAggregate> where TAggregate : class, IAggregateRoot
	{
		Task Save(IEvent domainEvent);
	}
}