using System;

namespace Theta.Platform.Messaging.Events
{
	public interface IEvent
	{
		Guid AggregateId { get; }

		string Type { get; }
	}
}