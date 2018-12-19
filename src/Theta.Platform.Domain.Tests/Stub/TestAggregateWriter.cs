using System;
using System.Collections.Generic;
using Theta.Platform.Domain.Tests.Stub.Events;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain.Tests.Stub
{
	public sealed class TestAggregateWriter : AggregateWriter<TestAggregateRoot>
	{
		private readonly Dictionary<string, Type> _eventTypes = new Dictionary<string, Type>
		{
			{ typeof(TestAggregateCreatedEvent).Name, typeof(TestAggregateCreatedEvent) },
			{ typeof(TestAggregateUpdatedEvent).Name, typeof(TestAggregateUpdatedEvent) }
		};

		public TestAggregateWriter(
			IEventPersistenceClient eventPersistenceClient, 
			IEventStreamingClient eventStreamingClient) : 
			base(eventPersistenceClient, eventStreamingClient)
		{
		}

		public override Dictionary<string, Type> GetEventTypes()
		{
			return _eventTypes;
		}
	}
}