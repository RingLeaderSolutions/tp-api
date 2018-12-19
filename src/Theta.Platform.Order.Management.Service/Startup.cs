using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Theta.Platform.Domain;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Messaging.EventStore;
using Theta.Platform.Messaging.EventStore.Configuration;
using Theta.Platform.Messaging.EventStore.Factories;
using Theta.Platform.Messaging.ServiceBus;
using Theta.Platform.Messaging.ServiceBus.Configuration;
using Theta.Platform.Messaging.ServiceBus.Factories;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Domain;
using Theta.Platform.Order.Management.Service.Messaging;
using Theta.Platform.Order.Management.Service.Messaging.Subscribers;

namespace Theta.Platform.Order.Management.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {
            var serviceBusConfiguration = new ServiceBusConfiguration();

            //Configuration.GetSection("PubSub").Bind(pubSubConfiguration);

            services.AddSingleton<IServiceBusConfiguration>(serviceBusConfiguration);
            services.AddSingleton<IServiceBusManagementConfiguration>(serviceBusConfiguration);
			
	        var commandTypeDictionary = new Dictionary<string, Type>();
	        AddCommandType(commandTypeDictionary, typeof(CreateOrderCommand));
	        AddCommandType(commandTypeDictionary, typeof(CompleteOrderCommand));
	        AddCommandType(commandTypeDictionary, typeof(PickupOrderCommand));
	        AddCommandType(commandTypeDictionary, typeof(PutDownOrderCommand));
	        AddCommandType(commandTypeDictionary, typeof(RejectOrderCommand));
	        AddCommandType(commandTypeDictionary, typeof(FillOrderCommand));
	        AddCommandType(commandTypeDictionary, typeof(RegisterSupplementaryEvidenceCommand));

			var commandQueueClient = new ServiceBusCommandQueueClient(
				commandTypeDictionary,
                new ServiceBusNamespaceFactory(serviceBusConfiguration),
                new QueueClientFactory(serviceBusConfiguration));

	        services.AddSingleton<ICommandQueueClient>(commandQueueClient);

			// event store
			var eventStoreConfiguration = new EventStoreConfiguration();
	        Configuration.GetSection("EventStore").Bind(eventStoreConfiguration);
	        services.AddSingleton<IEventStoreConfiguration>(eventStoreConfiguration);

	        EventStoreConnectionFactory factory = new EventStoreConnectionFactory(eventStoreConfiguration);
	        services.AddSingleton<IEventStoreConnectionFactory>(factory);
	        var eventStoreClient = new EventStoreClient(factory);
	        services.AddSingleton<IEventStreamingClient>(eventStoreClient);
	        services.AddSingleton<IEventPersistenceClient>(eventStoreClient);

	        services.AddSingleton<IAggregateWriter<Domain.Order>, OrderAggregateWriter>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            ConfigureAuthService(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Orders API", Version = "v1" });
            });
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // TODO: Implement authentication
        }

	    private List<ISubscriber<T>> GetSubscribers<T>(IApplicationBuilder app) where T : ICommand
	    {
		    return app.ApplicationServices.GetServices<ISubscriber<T>>()
			    .ToList();
	    }
		
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(
			IApplicationBuilder app, 
			IHostingEnvironment env, 
			IEventStoreConnection eventStoreConnection, 
			ServiceBusCommandQueueClient commandQueueClient,
			IAggregateWriter<Domain.Order> aggregateWriter)
        {
            await commandQueueClient.CreateQueueIfNotExists("order-service");

			// Connect to Eventstore
			await eventStoreConnection.ConnectAsync();

			var createSubscribers = GetSubscribers<CreateOrderCommand>(app);

			var subscriberDictionary = new Dictionary<string, List<ISubscriber<CreateOrderCommand>>>();

			// TODO: register this instead
			ISubscriber<CreateOrderCommand> createOrderSubscriber = new CreateOrderSubscriber(aggregateWriter);

			subscriberDictionary.Add(typeof(CreateOrderCommand).Name, new List<ISubscriber<CreateOrderCommand>>{ createOrderSubscriber });

			var subscription = commandQueueClient.Subscribe("order-service")
			 .Subscribe(
			  command =>
			  {
				  if (subscriberDictionary.TryGetValue(command.ReceivedCommand.Type, out List<ISubscriber<CreateOrderCommand>> subscribers))
				  {
					  var strongCommand = (CreateOrderCommand) command.ReceivedCommand;
					  subscribers.ForEach(s => s.Handle(strongCommand));
				  }
			  });

			app.UseCors("CorsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API V1");
            });
        }

        private async Task<IServiceBusNamespace> GetNamespace(IServiceBusManagementConfiguration config)
        {
            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(
                    config.ClientId,
                    config.ClientSecret,
                    config.TenantId,
                    AzureEnvironment.AzureGlobalCloud);

            var manager = ServiceBusManager.Authenticate(
                credentials,
                config.SubscriptionId);

            return await manager.Namespaces
                .GetByResourceGroupAsync(
                    config.ResourceGroup,
                    config.ResourceName);
        }

	    private static void AddCommandType(Dictionary<string, Type> collection, Type type)
	    {
		    collection.Add(type.Name, type);
	    }
	}
}
