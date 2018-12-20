using System;

namespace Theta.Platform.Messaging.Events
{
	public interface IEvent
	{
		Guid EventId { get; set; }

		Guid AggregateId { get; }

		string Type { get; }
	}
}