using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Theta.Platform.UI.Pricing.Streaming.Hubs;
using Theta.Platform.UI.Pricing.Streaming.Services;

namespace Theta.Platform.UI.Pricing.Streaming
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        // TODO: Ben to investigate CORS issues after core 2.2 upgrade:
                        // https://docs.microsoft.com/en-gb/aspnet/core/migration/21-to-22?view=aspnetcore-2.2&tabs=visual-studio#update-cors-policy
                        .WithOrigins(
                            "http://localhost:3000",
                            "http://omega-ui.azurewebsites.net")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                    );
            });

            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            ConfigureAuthService(services);

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
            // TODO - add any dependency health checks here with the tag "dependency"

            services.AddSingleton<RandomPriceGenerator>();
            services.AddSingleton<RandomNotificationGenerator>();
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // TODO: Implement authentication
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            //app.UseAuthentication();

            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<PricesHub>("/hub", options =>
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
                routes.MapHub<NotificationsHub>("/hubNotifications", options =>
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
            });

            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self") || r.Tags.Contains("dependency")
            });

            app.ApplicationServices.GetService<RandomPriceGenerator>()
                .GeneratePrices();


            app.ApplicationServices.GetService<RandomNotificationGenerator>()
                .GenerateNotifications();
        }
    }
}
