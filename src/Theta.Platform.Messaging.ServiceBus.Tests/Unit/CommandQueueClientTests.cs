using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using NUnit.Framework;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.ServiceBus.Factories;

namespace Theta.Platform.Messaging.ServiceBus.Tests.Unit
{
	[TestFixture]
	public class CommandQueueClientTests
	{
		private ServiceBusCommandQueueClient _commandQueueClient;
		private IQueueClient _queueClient;
		private IServiceBusNamespace _serviceBusNamespace;
		private Func<Message, CancellationToken, Task> _messageHandler = null;

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

			A.CallTo(() => _queueClient.RegisterMessageHandler(
					A<Func<Message, CancellationToken, Task>>.Ignored,
					A<MessageHandlerOptions>.Ignored))
				.Invokes((Func<Message, CancellationToken, Task> callback, MessageHandlerOptions options) => _messageHandler = callback);
		}

		[Test]
		public async Task SubscribePumpsNewObservableMessagesFromMessageHandler()
		{
			var receivedMessages = new List<IActionableMessage<ICommand>>();
			_commandQueueClient
				.Subscribe("test-queue")
				.Subscribe(message => receivedMessages.Add(message));

			A.CallTo(() => _queueClient.RegisterMessageHandler(
					A<Func<Message, CancellationToken, Task>>.Ignored,
					A<MessageHandlerOptions>.Ignored))
				.MustHaveHappenedOnceExactly();
			Assert.IsNotNull(_messageHandler, "Failed to capture the message handler");

			var command = new TestingCommand("bar");

			await SimulateCommandDelivery(command);

			Assert.AreEqual(1, receivedMessages.Count);
			Assert.AreEqual(command.Foo, ((TestingCommand)receivedMessages[0].ReceivedCommand).Foo);
		}

		[Test]
		public void ReceivingCommandWithNullTypeDeadLettersMessage()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void DisposingSubscriptionClosesQueueClientConnection()
		{
			var receivedMessages = new List<IActionableMessage<ICommand>>();
			var subscription = _commandQueueClient
				.Subscribe("test-queue")
				.Subscribe(message => receivedMessages.Add(message));

			A.CallTo(() => _queueClient.CloseAsync())
				.MustNotHaveHappened();

			subscription.Dispose();

			A.CallTo(() => _queueClient.CloseAsync())
				.MustHaveHappenedOnceExactly();
		}
		
		[Test]
		public async Task FailureToDeserializeCommandDeadLettersMessageAndDoesNotOnErrorStream()
		{
			var receivedMessages = new List<IActionableMessage<ICommand>>();
			var receivedErrors = new List<Exception>();

			_commandQueueClient
				.Subscribe("test-queue")
				.Subscribe(
					message => receivedMessages.Add(message),
					error => receivedErrors.Add(error));

			var invalidJson = "{$#@$#@}{{{{{";
			var messageBytes = Encoding.UTF8.GetBytes(invalidJson);
			var msg = new Message(messageBytes);

			// This is a hack to set the sequenceNumber on the SystemProperties collection, to avoid an exception that gets thrown
			// if the Message detects that it has not yet been delivered. Sadly there are internals all over the show, so we have no choice.
			var prop = msg.SystemProperties.GetType().GetField("sequenceNumber", System.Reflection.BindingFlags.NonPublic
			                                      | System.Reflection.BindingFlags.Instance);
			prop.SetValue(msg.SystemProperties, 0);

			await _messageHandler(msg, CancellationToken.None);

			// Ensure that we have not received an OnError, as this would cause the stream to die
			Assert.AreEqual(0, receivedErrors.Count);
			A.CallTo(() => _queueClient.DeadLetterAsync(A<string>.That.Not.IsNullOrEmpty(), A<string>.Ignored, A<string>.Ignored))
				.MustHaveHappenedOnceExactly();

			// Pump a successful message through and ensure that it is received successfully
			await SimulateCommandDelivery(new TestingCommand("bar"));
			Assert.AreEqual(1, receivedMessages.Count);
		}

		[Test]
		public async Task SendingMessageSerializesEncodesMessageAndCallsSendOnClient()
		{
			Message capturedMessage = null;
			A.CallTo(() => _queueClient.SendAsync(A<Message>.Ignored))
				.Invokes((Message message) => capturedMessage = message);

			var command = new TestingCommand("bar");
			
			await _commandQueueClient.Send("test-queue", command);

			var json = JsonConvert.SerializeObject(command);
			var bytes = Encoding.UTF8.GetBytes(json);

			A.CallTo(() => _queueClient.SendAsync(A<Message>.Ignored))
				.MustHaveHappenedOnceExactly();

			Assert.IsNotNull(capturedMessage, "Failed to capture the sent message");
			Assert.AreEqual(bytes, capturedMessage.Body);
		}

		private async Task SimulateCommandDelivery(ICommand command)
		{
			var json = JsonConvert.SerializeObject(command);
			var bytes = Encoding.UTF8.GetBytes(json);

			await _messageHandler(new Message(bytes), CancellationToken.None);
		}

		private class TestingCommand : ICommand
		{
			public TestingCommand(string foo)
			{
				Foo = foo;
			}

			public string Foo { get; }

            public string Type => this.GetType().Name;
        }
	}
}