using Theta.Platform.Domain.Tests.Stub.Events;

namespace Theta.Platform.Domain.Tests.Stub
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public sealed class TestAggregateRoot : AggregateRoot
	{
		public string Foo { get; private set; }

		public TestAggregateRoot()
		{
			Register<TestAggregateCreatedEvent>(When);
			Register<TestAggregateUpdatedEvent>(When);
		}

		private void When(TestAggregateCreatedEvent createdEvent)
		{
			Id = createdEvent.AggregateId;
			Foo = createdEvent.Foo;
		}

		private void When(TestAggregateUpdatedEvent updatedEvent)
		{
			Foo = updatedEvent.Foo;
		}
	}
}