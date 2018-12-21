using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Theta.Platform.Common.SecretManagement;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Messaging.EventStore;
using Theta.Platform.Messaging.EventStore.Configuration;
using Theta.Platform.Messaging.EventStore.Factories;
using Theta.Platform.Messaging.ServiceBus;
using Theta.Platform.Messaging.ServiceBus.Configuration;
using Theta.Platform.Messaging.ServiceBus.Factories;
using Theta.Platform.UI.Orders.API.Configuration;
using Theta.Platform.UI.Orders.API.Domain.Events;
using Theta.Platform.UI.Orders.API.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Theta.Platform.UI.Orders.API
{
    public class Startup
    {
        bool running = true;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	        var orderApiConfiguration = new OrderApiConfiguration();
	        Configuration.GetSection("OrderAPI").Bind(orderApiConfiguration);
	        services.AddSingleton(orderApiConfiguration);

			ConfigureEventStore(services);
			ConfigureServiceBus(services);

	        services.AddSingleton<IOrderReader, OrderReader>();
	        services.AddSingleton<IHostedService, OrderService>();

			// TODO - vault name automatically determined via convention + detection of environment name
			services.AddTransient<IVault, Vault>(x => new Vault("theta-dev-platform-vault"));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Orders API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
			});

            services.AddHealthChecks()
                .AddCheck("self", () => running ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy())
                .AddAsyncCheck("dummy-dependency", async () => { await Task.Delay(2000); return HealthCheckResult.Healthy(); }, new[] { "dependency" });
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IOrderReader orderReader)
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

            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self") || r.Tags.Contains("dependency")
            });

            // TODO: Remove this temporary method used to prove health checks work with k8s
            app.Map("/switch", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    running = !running;
                    await context.Response.WriteAsync($"{Environment.MachineName} running {running}");
                });
            });
		}

        private void ConfigureEventStore(IServiceCollection services)
        {
			var eventStoreConfiguration = new EventStoreConfiguration();
			Configuration.GetSection("EventStore").Bind(eventStoreConfiguration);
			services.AddSingleton<IEventStoreConfiguration>(eventStoreConfiguration);

	        IEventStoreConnectionFactory factory = new EventStoreConnectionFactory(eventStoreConfiguration);
	        services.AddSingleton(factory);

	        var eventStoreClient = new EventStoreClient(SubscribedEventTypes, factory);
	        services.AddSingleton<IEventStreamingClient>(eventStoreClient);
	        services.AddSingleton<IEventPersistenceClient>(eventStoreClient);
		}

        private void ConfigureServiceBus(IServiceCollection services)
        {
	        var serviceBusConfiguration = new ServiceBusConfiguration();
	        Configuration.GetSection("ServiceBus").Bind(serviceBusConfiguration);

	        services.AddSingleton<IServiceBusConfiguration>(serviceBusConfiguration);
	        services.AddSingleton<IServiceBusManagementConfiguration>(serviceBusConfiguration);

	        var commandQueueClient = new ServiceBusCommandQueueClient(
		        null,
		        new ServiceBusNamespaceFactory(serviceBusConfiguration),
		        new QueueClientFactory(serviceBusConfiguration));
	        services.AddSingleton<ICommandQueueClient>(commandQueueClient);
		}

		private Dictionary<string, Type> SubscribedEventTypes { get; } = new List<KeyValuePair<string, Type>>
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

        private static KeyValuePair<string, Type> CreateEventNameToTypeMapping(Type type)
        {
	        return new KeyValuePair<string, Type>(type.Name, type);
        }
	}
}
