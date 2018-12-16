using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.WindowsAzure.Storage.Table;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Messaging.Subscribers;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Theta.Platform.Order.Management.Service.Messaging;
using Theta.Platform.Order.Management.Service.Domain.Commands;
using Theta.Platform.Order.Management.Service.Framework;
using EventStore.ClientAPI;
using System.Net;

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
        public void ConfigureServices(IServiceCollection services)
        {
            var pubSubConfiguration = new PubSubConfiguration();
            Configuration.GetSection("PubSub").Bind(pubSubConfiguration);
            services.AddSingleton<IPubSubConfiguration>(pubSubConfiguration);

            var eventStoreConnection = EventStoreConnection.Create(
                ConnectionSettings.Default,
                new IPEndPoint(IPAddress.Loopback, 1113));

            services.AddSingleton<IEventStoreConnection>(eventStoreConnection);

            services.AddTransient<IPubsubResourceManager, PubsubResourceManager>();
            services.AddTransient<IAggregateRepository, AggregateRepository>();

            services.AddTransient<ISubscriber<CreateOrderCommand>, CreateOrderSubscriber>();
            services.AddTransient<ISubscriber<CompleteOrderCommand>, CompleteOrderSubscriber>();
            services.AddTransient<ISubscriber<PickupOrderCommand>, PickupOrderSubscriber>();
            services.AddTransient<ISubscriber<PutDownOrderCommand>, PutDownOrderSubscriber>();
            services.AddTransient<ISubscriber<RejectOrderCommand>, RejectOrderSubscriber>();
            services.AddTransient<ISubscriber<FillOrderCommand>, FillOrderSubscriber>();
            services.AddTransient<ISubscriber<RegisterSupplementaryEvidenceCommand>, RRegisterSupplementaryEvidenceCommandSubscriber>();

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IEventStoreConnection eventStoreConnection)
        {
            // Connect to Eventstore
            await eventStoreConnection.ConnectAsync();

            // Register the recievers
            app.ApplicationServices.GetServices<ISubscriber<CreateOrderCommand>>()
                .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            app.ApplicationServices.GetServices<ISubscriber<CompleteOrderCommand>>()
                .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            app.ApplicationServices.GetServices<ISubscriber<RejectOrderCommand>>()
                .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            app.ApplicationServices.GetServices<ISubscriber<PickupOrderCommand>>()
                .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            app.ApplicationServices.GetServices<ISubscriber<PutDownOrderCommand>>()
                .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            app.ApplicationServices.GetServices<ISubscriber<FillOrderCommand>>()
                .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            app.ApplicationServices.GetServices<ISubscriber<RegisterSupplementaryEvidenceCommand>>()
                .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());


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
    }
}
