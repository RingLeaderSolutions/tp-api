using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Theta.Platform.Domain.Tests.Stub;
using Theta.Platform.Domain.Tests.Stub.Events;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain.Tests
{
	[TestFixture]
	public class AggregateWriterTests
	{
		private readonly Subject<IEvent> _allEventsSubject = new Subject<IEvent>();

		private IEventPersistenceClient _persistenceClient;
		private IEventStreamingClient _streamingClient;

		[SetUp]
		public void Setup()
		{
			_persistenceClient = A.Fake<IEventPersistenceClient>();
			_streamingClient = A.Fake<IEventStreamingClient>();

			A.CallTo(() => _streamingClient.SubscribeToAll())
				.Returns(_allEventsSubject.AsObservable());
		}

		[Test]
		public async Task WhenSavingNewEventUsesVersionOfAggregateAsExpectedVersion()
		{
			var createdEvent = new TestAggregateCreatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1_Initial");
			var updatedEvent = new TestAggregateUpdatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1_Updated");
			A.CallTo(() => _persistenceClient.Retrieve(typeof(TestAggregateCreatedEvent)))
				.Returns(new IEvent[] { createdEvent });
			A.CallTo(() => _persistenceClient.Retrieve(typeof(TestAggregateUpdatedEvent)))
				.Returns(new IEvent[] { updatedEvent });

			var writer = new TestAggregateWriter(_persistenceClient, _streamingClient);
			await writer.StartAsync();

			var newUpdatedEvent = new TestAggregateUpdatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1_Updated_SecondTime");

			await writer.Save(newUpdatedEvent);

			A.CallTo(() => _persistenceClient.Save(
					"TestAggregateRoot_b166fbb3-b7f9-49d2-8a61-0b4df5fff561", 
					1, 
					A<IEvent>.Ignored))
				.MustHaveHappenedOnceExactly();
		}

		[Test]
		public async Task OnSaveIfSaveFailsEventIsNotPublished()
		{
			A.CallTo(() => _persistenceClient.Save(A<string>.Ignored, A<int>.Ignored, A<IEvent>.Ignored))
				.Throws(new Exception("Saving failed"));

			var writer = new TestAggregateWriter(_persistenceClient, _streamingClient);
			await writer.StartAsync();

			var newUpdatedEvent = new TestAggregateUpdatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1_Updated_SecondTime");

			try
			{
				await writer.Save(newUpdatedEvent);
			}
			catch (Exception)
			{
				// ignored
			}

			A.CallTo(() => _streamingClient.Publish(A<IEvent>.Ignored))
				.MustNotHaveHappened();
		}

		[Test]
		public async Task OnSaveEventPublishIsCalled()
		{
			var writer = new TestAggregateWriter(_persistenceClient, _streamingClient);
			await writer.StartAsync();

			var createdEvent = new TestAggregateCreatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1");
			await writer.Save(createdEvent);

			A.CallTo(() => _persistenceClient.Save(
					A<string>.Ignored,
					A<int>.Ignored,
					A<IEvent>.Ignored))
				.MustHaveHappenedOnceExactly();

			A.CallTo(() => _streamingClient.Publish(A<IEvent>.Ignored))
				.MustHaveHappenedOnceExactly();
		}
	}
}