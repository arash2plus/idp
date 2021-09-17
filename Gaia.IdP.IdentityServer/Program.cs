using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Gaia.IdP.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
                environmentName = "Production";
                
            var hostBuilder = Host
                .CreateDefaultBuilder(args)
                .UseEnvironment(environmentName)
                .ConfigureWebHostDefaults(webBuilder =>
                {
			        Serilog.Debugging.SelfLog.Enable(Console.Error);

                    webBuilder.UseSerilog((hostingContext, loggerConfiguration) => {
                        loggerConfiguration
                            .ReadFrom.Configuration(hostingContext.Configuration)
                            .Enrich.FromLogContext()
                            .Enrich.WithMachineName()
                            .Enrich.WithClientIp()
                            .Enrich.WithClientAgent();
                        });

                    webBuilder.UseStartup<Startup>();
                });

            return hostBuilder;
        }
    }
}