﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
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
using Theta.Platform.RFQ.Management.Service.Configuration;
using Theta.Platform.RFQ.Management.Service.Domain;
using Theta.Platform.RFQ.Management.Service.Domain.Commands;
using Theta.Platform.RFQ.Management.Service.Domain.Events;
using Theta.Platform.RFQ.Management.Service.Messaging;
using Theta.Platform.RFQ.Management.Service.Messaging.Subscribers;
using Theta.Platform.RFQ.Management.Service.QuoteManagement;

namespace Theta.Platform.RFQ.Management.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
            services.AddSingleton<RequestForQuotesAggregateWriter>();
            services.AddSingleton<IAggregateWriter<RequestForQuotes>>(i => i.GetService<RequestForQuotesAggregateWriter>());
            services.AddSingleton<IAggregateReader<RequestForQuotes>>(i => i.GetService<RequestForQuotesAggregateWriter>());

            // Retrieve and register all implementations of ISubscriber<>
            GetImplementations(typeof(ISubscriber<ICommand, IEvent>))
                .ForEach(type =>
                {
                    services.Add(new ServiceDescriptor(typeof(ISubscriber<ICommand, IEvent>), type, ServiceLifetime.Transient));
                });

            services.AddSingleton<IHostedService, RequestForQuotesSubscriber>();
            services.AddSingleton<IQuoteProvider, QuoteProvider>();

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
                c.SwaggerDoc("v1", new Info { Title = "RFQ Management Service API", Version = "v1" });
            });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
            // TODO - add any dependency health checks here with the tag "dependency"
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RFQ API V1");
            });

            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self") || r.Tags.Contains("dependency")
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
            CreateEventNameToTypeMapping(typeof(RFQCancelledEvent)),
            CreateEventNameToTypeMapping(typeof(RFQRaisedEvent)),
            CreateEventNameToTypeMapping(typeof(RFQQuoteReceivedEvent)),
            CreateEventNameToTypeMapping(typeof(RFQQuoteRetractedEvent))
        }.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        private static Dictionary<string, Type> CommandTypes { get; } = new List<KeyValuePair<string, Type>>
        {
            CreateEventNameToTypeMapping(typeof(CancelRFQCommand)),
            CreateEventNameToTypeMapping(typeof(RaiseRFQCommand))
        }.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        private static KeyValuePair<string, Type> CreateEventNameToTypeMapping(Type type)
        {
            return new KeyValuePair<string, Type>(type.Name, type);
        }
    }
}
