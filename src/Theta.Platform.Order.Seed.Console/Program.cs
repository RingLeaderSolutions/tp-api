using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.ServiceBus;
using Theta.Platform.Messaging.ServiceBus.Factories;
using Theta.Platform.Order.Seed.Console.Configuration;

namespace Theta.Platform.Order.Seed.Console
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var serviceBusConfiguration = new ServiceBusConfiguration();
            var section = configuration.GetSection("ServiceBus");
            section.Bind(serviceBusConfiguration);

            var orderQueueClient = new ServiceBusCommandQueueClient(
	            new Dictionary<string, Type>(), 
	            new ServiceBusNamespaceFactory(serviceBusConfiguration),
	            new QueueClientFactory(serviceBusConfiguration));

            var rfqQueueClient = new ServiceBusCommandQueueClient(
                new Dictionary<string, Type>(),
                new ServiceBusNamespaceFactory(serviceBusConfiguration),
                new QueueClientFactory(serviceBusConfiguration));

            Dictionary<string, ICommandQueueClient> ccqs = new Dictionary<string, ICommandQueueClient>
            {
                { "order-service", orderQueueClient },
                { "rfq-service", rfqQueueClient }
            };

            MainAsync(ccqs).GetAwaiter().GetResult();
        }

        static async Task MainAsync(Dictionary<string, ICommandQueueClient> ccqs)
        {
            DatastoreInitializer initializer = new DatastoreInitializer(ccqs);

            System.Console.WriteLine("Press any key to seed!");
            System.Console.ReadKey();

			await initializer.Seed();

            System.Console.WriteLine("Seeded! Press any key to exit.");
            System.Console.ReadKey();
        }
    }
}
