using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Theta.Platform.Domain.Tests.Stub;
using Theta.Platform.Domain.Tests.Stub.Events;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain.Tests
{
	[TestFixture]
	public class AggregateReaderTests
	{
		private readonly Subject<IEvent> _allEventsSubject = new Subject<IEvent>();

		private IEventPersistenceClient _persistenceClient;
		private IEventStreamingClient _streamingClient;
		
		[SetUp]
		public void Setup()
		{
			_persistenceClient = A.Fake<IEventPersistenceClient>();
			_streamingClient = A.Fake<IEventStreamingClient>();

			A.CallTo(() => _streamingClient.GetAllEventsStream())
				.Returns(_allEventsSubject.AsObservable());

			A.CallTo(() => _streamingClient.ConnectionStateChanged)
				.Returns(Observable.Return(StreamingConnectionState.Connected));
		}

		[Test]
		public async Task ExistingEventsAreLoadedAndProcessedOnStart()
		{
			var existingEvent = new TestAggregateCreatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1");
			A.CallTo(() => _persistenceClient.Retrieve(typeof(TestAggregateCreatedEvent)))
				.Returns(new IEvent[] { existingEvent });

			var reader = new TestAggregateReader(_persistenceClient, _streamingClient);
			await reader.StartAsync();

			var aggregates = reader.Get();
			Assert.AreEqual(1, aggregates.Length);

			var actualAggregate = aggregates[0];
			Assert.AreEqual(existingEvent.AggregateId, actualAggregate.Id);
			Assert.AreEqual(existingEvent.Foo, actualAggregate.Foo);
		}

		[Test]
		public async Task StreamingEventsAreBufferedUntilStateOfTheWorldIsRetrieved()
		{
			var mre = new ManualResetEvent(false);
			var existingEvent = new TestAggregateCreatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1");
			A.CallTo(() => _persistenceClient.Retrieve(typeof(TestAggregateCreatedEvent)))
				.Returns(Task.Run(() =>
				{
					mre.WaitOne();
					return new IEvent[] { existingEvent };
				}));

			var reader = new TestAggregateReader(_persistenceClient, _streamingClient);
			await Task.Factory.StartNew(async () =>
			{
				await reader.StartAsync();
				Assert.AreEqual(2, reader.Get().Length);

				_allEventsSubject.OnNext(new TestAggregateCreatedEvent(new Guid("15dd702a-7106-434e-9aa0-2f8f67fdd85d"), "Test3"));
				Assert.AreEqual(3, reader.Get().Length);
			});

			Assert.AreEqual(0, reader.Get().Length);

			_allEventsSubject.OnNext(new TestAggregateCreatedEvent(new Guid("db8cedf3-a4f8-4d0c-835b-e05692d69937"), "Test2"));

			Assert.AreEqual(0, reader.Get().Length);

			mre.Set();
		}

		[Test]
		public async Task ExistingEventsWithSameAggregateIdAreAppliedToSameAggregate()
		{
			var createdEvent = new TestAggregateCreatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1_Initial");
			var updatedEvent = new TestAggregateUpdatedEvent(new Guid("b166fbb3-b7f9-49d2-8a61-0b4df5fff561"), "Test1_Updated");
			A.CallTo(() => _persistenceClient.Retrieve(typeof(TestAggregateCreatedEvent)))
				.Returns(new IEvent[] { createdEvent });
			A.CallTo(() => _persistenceClient.Retrieve(typeof(TestAggregateUpdatedEvent)))
				.Returns(new IEvent[] { updatedEvent });

			var reader = new TestAggregateReader(_persistenceClient, _streamingClient);
			await reader.StartAsync();

			var aggregates = reader.Get();
			Assert.AreEqual(1, aggregates.Length);

			var actualAggregate = aggregates[0];
			Assert.AreEqual(updatedEvent.Foo, actualAggregate.Foo);
		}
	}
}
