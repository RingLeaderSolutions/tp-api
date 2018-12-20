using System;
using System.Collections.Generic;
using Theta.Platform.Domain.Tests.Stub.Events;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain.Tests.Stub
{
	public sealed class TestAggregateReader : AggregateReader<TestAggregateRoot>
	{
		public TestAggregateReader(
			IEventPersistenceClient eventPersistenceClient, 
			IEventStreamingClient eventStreamingClient) : 
			base(eventPersistenceClient, eventStreamingClient)
		{
		}

		protected override Dictionary<string, Type> SubscribedEventTypes { get; } = new Dictionary<string, Type>
		{
			{ typeof(TestAggregateCreatedEvent).Name, typeof(TestAggregateCreatedEvent) },
			{ typeof(TestAggregateUpdatedEvent).Name, typeof(TestAggregateUpdatedEvent) }
		};
	}
}