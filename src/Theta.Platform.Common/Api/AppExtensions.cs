using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Reflection;

namespace Theta.Platform.Common.Api
{
    public static class AppExtensions
    {
        public static void AddStatusEndpoint(this IApplicationBuilder app)
        {
            app.Map("/status", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var assembly = Assembly.GetEntryAssembly();
                    var assemblyVersion = assembly.GetName().Version;

                    var o = new { version = assemblyVersion.ToString() };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(o), System.Text.Encoding.UTF8);
                });
            });
        }
    }
}
