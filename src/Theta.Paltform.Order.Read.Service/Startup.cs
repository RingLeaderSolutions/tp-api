using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Theta.Paltform.Order.Read.Service.Configuration;
using Theta.Paltform.Order.Read.Service.Data;
using Theta.Platform.Messaging.Events;
using Theta.Platform.Messaging.EventStore;
using Theta.Platform.Messaging.EventStore.Configuration;
using Theta.Platform.Messaging.EventStore.Factories;

namespace Theta.Paltform.Order.Read.Service
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
            IEventStoreConfiguration eventStoreConfiguration = GetEventStoreConfiguration();
            services.AddSingleton<IEventStoreConfiguration>(eventStoreConfiguration);

            IEventStoreConnectionFactory factory = new EventStoreConnectionFactory(eventStoreConfiguration);
            services.AddSingleton<IEventStoreConnectionFactory>(factory);

            var eventStoreClient = new EventStoreClient(SubscribedEventTypes, factory);
            services.AddSingleton<IEventStreamingClient>(eventStoreClient);
            services.AddSingleton<IEventPersistenceClient>(eventStoreClient);

            services.AddSingleton<IOrderReader, OrderReader>();

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
            });
		}

        private IEventStoreConfiguration GetEventStoreConfiguration()
        {
            var eventStoreConfiguration = new EventStoreConfiguration();
            Configuration.GetSection("EventStore").Bind(eventStoreConfiguration);
            return eventStoreConfiguration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IOrderReader orderReader)
        {
	        app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
	            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Read Service API V1");
            });

            await orderReader.StartAsync();
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
