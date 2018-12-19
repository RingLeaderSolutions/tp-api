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
using Theta.Platform.Order.Management.Service.Data;
using Theta.Platform.Order.Management.Service.Configuration;
using Theta.Platform.Order.Management.Service.Messaging.Subscribers;
using Theta.Platform.Order.Management.Service.Messaging.MessageContracts;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
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
        public void ConfigureServices(IServiceCollection services)
        {
            var datastorageConfiguration = new DatastorageConfiguration();
            Configuration.GetSection("Datastorage").Bind(datastorageConfiguration);
            services.AddSingleton<IDatastorageConfiguration>(datastorageConfiguration);

            var pubSubConfiguration = new PubSubConfiguration();
            Configuration.GetSection("PubSub").Bind(pubSubConfiguration);
            services.AddSingleton<IPubSubConfiguration>(pubSubConfiguration);

            services.AddTransient<IPubsubResourceManager, PubsubResourceManager>();
            services.AddTransient<ISubscriber<Order, OrderCreatedEvent>, CreateOrderSubscriber>();
            services.AddSingleton<IAzureStorageResourceManager, AzureStorageResourceManager>();
            services.AddTransient<IOrderRepository, OrderRepository>();


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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IAzureStorageResourceManager azureStorageResourceManager)
        {
            // Setup the cloud storage table
            await azureStorageResourceManager.CreateOrdersTableAsync();

            // Register the recievers
            app.ApplicationServices.GetServices<ISubscriber<Order, OrderCreatedEvent>>()
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
