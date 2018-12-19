using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventStore.ClientAPI;
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

        public void ConfigureServices(IServiceCollection services)
        {
			// Configuration: ServiceBus
            var serviceBusConfiguration = new ServiceBusConfiguration();
            Configuration.GetSection("ServiceBus").Bind(serviceBusConfiguration);

            services.AddSingleton<IServiceBusConfiguration>(serviceBusConfiguration);
            services.AddSingleton<IServiceBusManagementConfiguration>(serviceBusConfiguration);

			// Configuration: EventStore
			var eventStoreConfiguration = new EventStoreConfiguration();
			Configuration.GetSection("EventStore").Bind(eventStoreConfiguration);
			services.AddSingleton<IEventStoreConfiguration>(eventStoreConfiguration);

			// ServiceBus registration
			var commandQueueClient = new ServiceBusCommandQueueClient(
				CommandTypes,
                new ServiceBusNamespaceFactory(serviceBusConfiguration),
                new QueueClientFactory(serviceBusConfiguration));
	        services.AddSingleton<ICommandQueueClient>(commandQueueClient);

			// EventStore registrations
            var eventStoreClient = new EventStoreClient(
	            new EventStoreConnectionFactory(eventStoreConfiguration));
	        services.AddSingleton<IEventStreamingClient>(eventStoreClient);
            services.AddSingleton<IEventPersistenceClient>(eventStoreClient);

            services.AddSingleton<IAggregateWriter<Domain.Order>, OrderAggregateWriter>();

			// Retrieve and register all implementations of ISubscriber<>
			GetImplementations(typeof(ISubscriber<ICommand, IEvent>))
				.ForEach(type =>
				{
					services.Add(new ServiceDescriptor(typeof(ISubscriber<ICommand, IEvent>), type, ServiceLifetime.Transient));
				});
			
			
            
            services.AddSingleton<IHostedService, OrderSubscriber>();

			// ASP.NET addons
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
			
            services.AddMvc()
	            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Orders API", Version = "v1" });
            });
        }
		
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API V1");
            });
        }

        private List<TypeInfo> GetImplementations(Type interfaceType)
        {
	        if (!interfaceType.IsInterface)
	        {
		        throw new InvalidOperationException($"Unable to get implementations: provided type [{interfaceType.Name}] is not an interface");
	        }

			// Retrieve all non-abstract implementations of the given interface type
	        return Assembly.GetExecutingAssembly().DefinedTypes
		        .Where(type => type.GetInterfaces().Contains(interfaceType) && !type.IsAbstract)
		        .ToList();
		}

        private static Dictionary<string, Type> CommandTypes
        {
            get
            {
                Dictionary<string, Type> commandTypeDictionary = new Dictionary<string, Type>();

                AddCommandType(commandTypeDictionary, typeof(CreateOrderCommand));
                AddCommandType(commandTypeDictionary, typeof(CompleteOrderCommand));
                AddCommandType(commandTypeDictionary, typeof(PickupOrderCommand));
                AddCommandType(commandTypeDictionary, typeof(PutDownOrderCommand));
                AddCommandType(commandTypeDictionary, typeof(RejectOrderCommand));
                AddCommandType(commandTypeDictionary, typeof(FillOrderCommand));
                AddCommandType(commandTypeDictionary, typeof(RegisterSupplementaryEvidenceCommand));

                return commandTypeDictionary;
            }
        }

        private static void AddCommandType(Dictionary<string, Type> collection, Type type)
        {
            collection.Add(type.Name, type);
        }
    }
}
