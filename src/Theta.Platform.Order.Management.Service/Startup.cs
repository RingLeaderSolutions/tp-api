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
using Theta.Platform.Common.Api;
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
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Domain.Events;
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
				EventTypes,
	            new EventStoreConnectionFactory(eventStoreConfiguration));
	        services.AddSingleton<IEventStreamingClient>(eventStoreClient);
            services.AddSingleton<IEventPersistenceClient>(eventStoreClient);

			// Register the AggregateWriter as the implementation of both IAggregateWriter and IAggregateReader
            services.AddSingleton<OrderAggregateWriter>();
            services.AddSingleton<IAggregateWriter<Domain.Order>>(i => i.GetService<OrderAggregateWriter>());
            services.AddSingleton<IAggregateReader<Domain.Order>>(i => i.GetService<OrderAggregateWriter>());

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
                c.SwaggerDoc("v1", new Info { Title = "Order Management Service API", Version = "v1" });
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
            app.AddStatusEndpoint();

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

        private static Dictionary<string, Type> EventTypes { get; } = new List<KeyValuePair<string, Type>>
        {
	        CreateEventNameToTypeMapping(typeof(OrderCreatedEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderCompletedEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderFilledEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderPickedUpEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderPickUpRejectedEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderPutDownEvent)),
	        CreateEventNameToTypeMapping(typeof(OrderRejectedEvent)),
	        CreateEventNameToTypeMapping(typeof(SupplementaryEvidenceReceivedEvent))
        }.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        private static Dictionary<string, Type> CommandTypes { get; } = new List<KeyValuePair<string, Type>>
        {
	        CreateEventNameToTypeMapping(typeof(CreateOrderCommand)),
	        CreateEventNameToTypeMapping(typeof(CompleteOrderCommand)),
	        CreateEventNameToTypeMapping(typeof(PickupOrderCommand)),
	        CreateEventNameToTypeMapping(typeof(PutDownOrderCommand)),
	        CreateEventNameToTypeMapping(typeof(RejectOrderCommand)),
	        CreateEventNameToTypeMapping(typeof(FillOrderCommand)),
	        CreateEventNameToTypeMapping(typeof(RegisterSupplementaryEvidenceCommand))
        }.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        private static KeyValuePair<string, Type> CreateEventNameToTypeMapping(Type type)
        {
	        return new KeyValuePair<string, Type>(type.Name, type);
        }
	}
}