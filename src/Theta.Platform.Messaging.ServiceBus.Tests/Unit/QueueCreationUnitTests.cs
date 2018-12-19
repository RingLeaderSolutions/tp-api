using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent.Queue.Definition;
using Microsoft.Azure.ServiceBus;
using NUnit.Framework;
using Theta.Platform.Messaging.ServiceBus.Factories;

namespace Theta.Platform.Messaging.ServiceBus.Tests.Unit
{
	[TestFixture]
	public class QueueCreationUnitTests
	{
		private ServiceBusCommandQueueClient _commandQueueClient;
		private IQueueClient _queueClient;
		private IServiceBusNamespace _serviceBusNamespace;

		[SetUp]
		public void Setup()
		{
			_serviceBusNamespace = A.Fake<IServiceBusNamespace>();
			_queueClient = A.Fake<IQueueClient>();

			var serviceBusNamespaceFactory = A.Fake<IServiceBusNamespaceFactory>();
			var queueClientFactory = A.Fake<IQueueClientFactory>();

			A.CallTo(() => serviceBusNamespaceFactory.Create())
				.Returns(_serviceBusNamespace);

			A.CallTo(() => queueClientFactory.Create(A<string>.Ignored))
				.Returns(_queueClient);

			_commandQueueClient = new ServiceBusCommandQueueClient(
                new Dictionary<string, Type>(),
                serviceBusNamespaceFactory,
				queueClientFactory);
		}

		[Test]
		public async Task CreatesQueueIfDoesNotAlreadyExist()
		{
			var existingQueueName = "existing-queue-name";
			var queues = StubNamespaceQueues(existingQueueName);
			var creationObject = StubNamespaceCreationMethods(queues);

			var newQueueName = "unknown-queue-name";
			await _commandQueueClient.CreateQueueIfNotExists(newQueueName);
			
			A.CallTo(() => creationObject.CreateAsync(A<CancellationToken>.Ignored, A<bool>.Ignored))
				.MustHaveHappenedOnceExactly();
		}

		[Test]
		public async Task DoesNotCreateQueueIfNameAlreadyExists()
		{
			var existingQueueName = "existing-queue-name";
			var queues = StubNamespaceQueues(existingQueueName);
			var creationObject = StubNamespaceCreationMethods(queues);

			await _commandQueueClient.CreateQueueIfNotExists(existingQueueName);

			A.CallTo(() => creationObject.CreateAsync(A<CancellationToken>.Ignored, A<bool>.Ignored))
				.MustNotHaveHappened();
		}

		private IQueues StubNamespaceQueues(params string[] queueNames)
		{
			// Construct a collection of fake queues.
			var fakeQueues = new List<IQueue>();
			foreach (var name in queueNames)
			{
				var fakeQueue = A.Fake<IQueue>();
				A.CallTo(() => fakeQueue.Name)
					.Returns(name);
				fakeQueues.Add(fakeQueue);
			}

			// Stub the namespace to return the fake queues.
			var queueCollection = A.Fake<IPagedCollection<IQueue>>();
			A.CallTo(() => queueCollection.GetEnumerator())
				.Returns(fakeQueues.GetEnumerator());

			var queues = A.Fake<IQueues>();
			A.CallTo(() => queues.ListAsync(A<bool>.Ignored, A<CancellationToken>.Ignored))
				.Returns(queueCollection);

			A.CallTo(() => _serviceBusNamespace.Queues)
				.Returns(queues);

			return queues;
		}

		private IWithCreate StubNamespaceCreationMethods(IQueues queues)
		{
			// Stub fluent methods used for creation
			var blank = A.Fake<IBlank>();
			A.CallTo(() => queues.Define(A<string>.Ignored))
				.Returns(blank);

			var withCreate = A.Fake<IWithCreate>();
			A.CallTo(() => blank.WithSizeInMB(A<long>.Ignored))
				.Returns(withCreate);

			A.CallTo(() => withCreate.WithMessageMovedToDeadLetterQueueOnMaxDeliveryCount(A<int>.Ignored))
				.Returns(withCreate);
			A.CallTo(() => withCreate.WithDefaultMessageTTL(A<TimeSpan>.Ignored))
				.Returns(withCreate);

			return withCreate;
		}
	}
}