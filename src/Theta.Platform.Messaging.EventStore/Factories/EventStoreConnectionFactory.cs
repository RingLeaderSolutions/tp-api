﻿using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Theta.Platform.Messaging.EventStore.Configuration;

namespace Theta.Platform.Messaging.EventStore.Factories
{
	public sealed class EventStoreConnectionFactory : IEventStoreConnectionFactory
	{
		private readonly IEventStoreConfiguration _configuration;

		public EventStoreConnectionFactory(IEventStoreConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IEventStoreConnection Create()
		{
            var setting = ConnectionSettings.Create().SetDefaultUserCredentials(
                new UserCredentials(_configuration.Username, _configuration.Password));

            IEventStoreConnection eventStoreConnection = EventStoreConnection
                .Create(setting, _configuration.Endpoint);

            return eventStoreConnection;
        }
	}
}