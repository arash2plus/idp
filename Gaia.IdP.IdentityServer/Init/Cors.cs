using System;
using System.Linq;
using Gaia.IdP.IdentityServer.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class Cors
    {
        public static IServiceCollection AddCustomizedCors(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(options => {
                        options
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                });
            }
            else
            {
                var corsOptions = new CorsOptions(configuration);

                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(options => {
                        options
                            .WithOrigins(corsOptions.AllowedOrigins.ToArray())
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                });
            }

            return services;
        }
    }
}
