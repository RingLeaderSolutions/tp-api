﻿using System;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Events;

namespace Theta.Platform.Domain
{
	public abstract class AggregateWriter<TAggregate> : AggregateReader<TAggregate>, IAggregateWriter<TAggregate> where TAggregate : class, IAggregateRoot
	{
		protected AggregateWriter(
			IEventPersistenceClient eventPersistenceClient, 
			IEventStreamingClient eventStreamingClient) : 
			base(eventPersistenceClient, eventStreamingClient)
		{
		}

		// ReSharper disable once MemberCanBePrivate.Global
		protected string StreamName(Guid id)
		{
			return $"{typeof(TAggregate).Name}_{id}";
		}

		public async Task Save(IEvent domainEvent)
		{
			// Ensure the type of the event that is being saved is also one that we are interested in
			if (!SubscribedEventTypes.ContainsKey(domainEvent.Type))
			{
				// TODO: Logging
				throw new InvalidOperationException($"Unable to save event of type [{domainEvent.Type}] that is not expressed in GetEventTypes [EventTypes: {string.Join(", ", SubscribedEventTypes.Keys)}]");
			}

			// An expectedVersion of -1 indicates that the aggregate does not yet have any events associated with it
			var expectedVersion = -1;
			if (aggregateCache.TryGetValue(domainEvent.AggregateId, out TAggregate existingAggregate))
			{
				expectedVersion = existingAggregate.Version;
			}

			await _eventPersistenceClient.Save(StreamName(domainEvent.AggregateId), expectedVersion, domainEvent);
			await _eventStreamingClient.Publish(domainEvent);
		}
	}
}