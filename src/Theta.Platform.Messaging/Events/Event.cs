using System;

namespace Theta.Platform.Messaging.Events
{
	public abstract class Event : IEvent
	{
		protected Event(Guid aggregateId)
		{
			AggregateId = aggregateId;
		}

		public Guid EventId { get; set; }

		public Guid AggregateId { get; }

		public string Type => this.GetType().Name;
	}
}