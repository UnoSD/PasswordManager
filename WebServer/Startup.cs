using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paccia;
using System;
using WebServer.Controllers;
using System.Collections.Generic;

namespace WebServer
{
    public class Startup
    {
        const string CorsPolicyName = "Cors";

        public void ConfigureServices(IServiceCollection services)
        {
            void SetupOptions(HmacAuthenticationOptions options)
            {
                options.MaxRequestAge = TimeSpan.FromSeconds(300);
                options.AppId = "92C9C40B-BB2F-4813-8FA8-39628AD7EA4E";
                options.SecretKey = "CB6C5713-E213-4524-B93C-17020F0B5B4B".ToBase64();
            }

            var schemeName = HmacAuthenticationHandler.SchemeName;

            services.AddSingleton<AesEncryptorDecryptor>()
                    .AddSingleton<ShaHeaderGenerator>()
                    .AddSingleton<ISerializer<IEnumerable<Secret>>, JsonSerializer<IEnumerable<Secret>>>()
                    .AddSingleton<IEncryptor, ShaAesEncryptor>()
                    .AddSingleton<IDecryptor, ShaAesDecryptor>()
                    .AddSingleton<IConfigurationDefaults, WebServerHardcodedConfigurationDefaults>()
                    .AddSingleton<Paccia.IConfiguration, Configuration>()
                    .AddSingleton<Logger>()
                    .AddSingleton<EncryptionSerializersFactory<IEnumerable<Secret>>>()
                    .AddSingleton<EncryptedRepositoryFactory<Secret>>()
                    .AddSingleton(provider => provider.GetService<EncryptedRepositoryFactory<Secret>>()
                                                      .GetRepository("password".ToSecureString(), 
                                                                     Environment.MachineName, 
                                                                     ConfigurationKey.SecretsFilePath))
                    .AddScheme<HmacAuthenticationOptions, HmacAuthenticationHandler>(schemeName, SetupOptions)
                    .AddCors(options => options.AddPolicy(CorsPolicyName, AllowCorsSettings))
                    .AddMemoryCache()
                    .AddAuthentication(schemeName);

            services.AddMvc(s => s.ModelBinderProviders.Insert(0, ModelBinderProvider.From(new SecretModelBinder())));
        }

        static void AllowCorsSettings(CorsPolicyBuilder builder) =>
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();

        public void Configure(IApplicationBuilder builder, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            var configuration = GetConfiguration(environment);

            loggerFactory.AddConsole(configuration.GetSection("Logging"))
                         .AddDebug();

            builder.UseCors(CorsPolicyName)
                   .UseAuthentication()
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
