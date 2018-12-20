using System;
using System.Threading.Tasks;

namespace Theta.Platform.Messaging.Events
{
	public interface IEventStreamingClient : IDisposable
	{
		Task Connect();
		IObservable<IEvent> GetAllEventsStream();
		Task Publish(IEvent domainEvent);
		StreamingConnectionState ConnectionState { get; }
		IObservable<StreamingConnectionState> ConnectionStateChanged { get; }
	}
}