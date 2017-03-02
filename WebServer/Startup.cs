using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paccia;

namespace WebServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddAuthentication()
                    .AddMemoryCache()
                    .AddMvc();

        public void Configure(IApplicationBuilder builder, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            var configuration = GetConfiguration(environment);

            loggerFactory.AddConsole(configuration.GetSection("Logging"))
                         .AddDebug();

            var authenticationOptions = new HmacAuthenticationOptions
            {
                // TODO: Generate will all Base64 characters, not only Guid characters.
                SecretKey = "CB6C5713-E213-4524-B93C-17020F0B5B4B".ToBase64(),
                AppId = "92C9C40B-BB2F-4813-8FA8-39628AD7EA4E",
                AutomaticAuthenticate = true
            };

            builder.UseMiddleware<HmacAuthenticationMiddleware>(Options.Create(authenticationOptions))
                   .UseMvc();
        }

        static IConfigurationRoot GetConfiguration(IHostingEnvironment environment) =>
            new ConfigurationBuilder().SetBasePath(environment.ContentRootPath)
                                      .AddJsonFile("appsettings.json", true, true)
                                      .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true)
                                      .AddEnvironmentVariables()
                                      .Build();
    }
}
