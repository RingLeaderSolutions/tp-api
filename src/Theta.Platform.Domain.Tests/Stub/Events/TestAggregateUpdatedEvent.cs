using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain.Tests.Stub.Events
{
	public sealed class TestAggregateUpdatedEvent : Event
	{
		public TestAggregateUpdatedEvent(Guid aggregateId, string foo)
			: base(aggregateId)
		{
			Foo = foo;
			EventId = Guid.NewGuid();
		}

		public string Foo { get; }
	}
}