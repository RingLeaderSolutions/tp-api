using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

            var eventStoreClient = new EventStoreClient(factory);
            services.AddSingleton<IEventStreamingClient>(eventStoreClient);
            services.AddSingleton<IEventPersistenceClient>(eventStoreClient);

            services.AddSingleton<IOrderReader, OrderReader>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
            await orderReader.StartAsync();

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
        }
    }
}
