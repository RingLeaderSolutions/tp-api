using System;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain.Tests.Stub.Events
{
	public sealed class TestAggregateCreatedEvent : Event
	{
		public TestAggregateCreatedEvent(Guid aggregateId, string foo) 
			: base(aggregateId)
		{
			Foo = foo;
			EventId = Guid.NewGuid();
		}

		public string Foo { get; }
	}
}