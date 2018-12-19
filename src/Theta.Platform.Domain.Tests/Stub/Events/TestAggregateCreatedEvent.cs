using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain.Tests.Stub.Events
{
	public sealed class TestAggregateCreatedEvent : IEvent
	{
		public TestAggregateCreatedEvent(Guid aggregateId, string foo)
		{
			AggregateId = aggregateId;
			Foo = foo;
		}

		public string Foo { get; }

		public Guid AggregateId { get; }

		public string Type => this.GetType().Name;
	}
}