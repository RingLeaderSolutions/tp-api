using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Theta.Platform.Order.Seed.Console.Configuration;
using Theta.Platform.Order.Seed.Console.Messaging;

namespace Theta.Platform.Order.Seed.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Press any key to seed!");
            System.Console.ReadKey();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var pubSubConfiguration = new PubSubConfiguration();
            var section = configuration.GetSection("PubSub");
            section.Bind(pubSubConfiguration);

            TopicClientProvider topicClientProvider = new TopicClientProvider(pubSubConfiguration);

            MainAsync(topicClientProvider).GetAwaiter().GetResult();
        }

        static async Task MainAsync(TopicClientProvider topicClientProvider)
        {
            DatastoreInitializer initializer = new DatastoreInitializer(topicClientProvider);

            await initializer.Seed();

            System.Console.WriteLine("Seeded!");

            System.Console.ReadKey();
        }
    }
}
