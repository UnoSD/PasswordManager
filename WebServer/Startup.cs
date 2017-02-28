using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) => services.AddMvc();

        public void Configure(IApplicationBuilder builder, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            var configuration = GetConfiguration(environment);

            loggerFactory.AddConsole(configuration.GetSection("Logging"));

            loggerFactory.AddDebug();

            builder.UseMvc();
        }

        static IConfigurationRoot GetConfiguration(IHostingEnvironment environment) =>
            new ConfigurationBuilder().SetBasePath(environment.ContentRootPath)
                                      .AddJsonFile("appsettings.json", true, true)
                                      .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true)
                                      .AddEnvironmentVariables()
                                      .Build();
    }
}
