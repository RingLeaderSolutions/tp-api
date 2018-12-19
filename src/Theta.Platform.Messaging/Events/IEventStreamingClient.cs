using System;
using System.Threading.Tasks;

namespace Theta.Platform.Messaging.Events
{
	public interface IEventStreamingClient
	{
		Task<IObservable<IEvent>> SubscribeToAll();
		Task<IObservable<TEvent>> Subscribe<TEvent>() where TEvent : IEvent;
		Task<IObservable<IEvent>> Subscribe(string streamName);
		Task Publish(IEvent domainEvent);
	}
}