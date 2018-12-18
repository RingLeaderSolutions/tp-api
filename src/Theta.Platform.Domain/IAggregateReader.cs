using System;
using System.Threading.Tasks;

namespace Theta.Platform.Domain
{
	public interface IAggregateReader<out TAggregate> : IDisposable where TAggregate : class, IAggregateRoot
	{
		Task StartAsync();
		TAggregate GetById(Guid id);
		TAggregate[] Get();
	}
}