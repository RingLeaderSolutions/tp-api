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
using System.Net;
using System.Threading.Tasks;
using Theta.Platform.Messaging.Commands;
using Theta.Platform.Messaging.ServiceBus;
using Theta.Platform.Messaging.ServiceBus.Configuration;
using Theta.Platform.Messaging.ServiceBus.Factories;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Framework;
using Theta.Platform.Order.Management.Service.Messaging;

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

            var serviceBusNamespace = await GetNamespace(serviceBusConfiguration);

            var commandQueueClient = new ServiceBusCommandQueueClient(
                new ServiceBusNamespaceFactory(serviceBusConfiguration),
                new QueueClientFactory(serviceBusConfiguration));

            var setting = ConnectionSettings.Create()
                .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));

            var tcpEndPoint = new IPEndPoint(IPAddress.Loopback, 1113);
            IEventStoreConnection eventStoreConnection = EventStoreConnection
                .Create(setting, tcpEndPoint);

            services.AddSingleton<IEventStoreConnection>(eventStoreConnection);

            services.AddTransient<IPubsubResourceManager, PubsubResourceManager>();
            services.AddTransient<IAggregateRepository, AggregateRepository>();

            services.AddSingleton<ICommandQueueClient, ServiceBusCommandQueueClient>();

            //services.AddTransient<ISubscriber<CreateOrderCommand>, CreateOrderSubscriber>();
            //services.AddTransient<ISubscriber<CompleteOrderCommand>, CompleteOrderSubscriber>();
            //services.AddTransient<ISubscriber<PickupOrderCommand>, PickupOrderSubscriber>();
            //services.AddTransient<ISubscriber<PutDownOrderCommand>, PutDownOrderSubscriber>();
            //services.AddTransient<ISubscriber<RejectOrderCommand>, RejectOrderSubscriber>();
            //services.AddTransient<ISubscriber<FillOrderCommand>, FillOrderSubscriber>();
            //services.AddTransient<ISubscriber<RegisterSupplementaryEvidenceCommand>, RRegisterSupplementaryEvidenceCommandSubscriber>();

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
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IEventStoreConnection eventStoreConnection, ServiceBusCommandQueueClient commandQueueClient)
        {
            await commandQueueClient.CreateQueueIfNotExists("create-order");

            

            commandQueueClient.Subscribe<CreateOrderCommand>("create-order");

            // Connect to Eventstore
            // await eventStoreConnection.ConnectAsync();

            // Register the recievers
            //app.ApplicationServices.GetServices<ISubscriber<CreateOrderCommand>>()
            //    .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            //app.ApplicationServices.GetServices<ISubscriber<CompleteOrderCommand>>()
            //    .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            //app.ApplicationServices.GetServices<ISubscriber<RejectOrderCommand>>()
            //    .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            //app.ApplicationServices.GetServices<ISubscriber<PickupOrderCommand>>()
            //    .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            //app.ApplicationServices.GetServices<ISubscriber<PutDownOrderCommand>>()
            //    .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            //app.ApplicationServices.GetServices<ISubscriber<FillOrderCommand>>()
            //    .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());

            //app.ApplicationServices.GetServices<ISubscriber<RegisterSupplementaryEvidenceCommand>>()
            //    .ToList().ForEach(x => x.RegisterOnMessageHandlerAndReceiveMessages());


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
    }
}
