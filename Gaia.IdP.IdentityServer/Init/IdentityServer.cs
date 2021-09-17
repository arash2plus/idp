using Gaia.IdP.DomainModel.Models;
using Gaia.IdP.IdentityServer.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class IdentityServer
    {
        public static IServiceCollection AddCustomizedIdentityServer(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var origins = new OriginsOptions(configuration);
            var userInteraction = new IS4UserInteractionOptoins(configuration);

            var builder = services.AddIdentityServer(options =>
            {
                options.IssuerUri = origins.IdP;
                
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = false;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction.LoginUrl = $"{origins.IdPClient}/{userInteraction.LoginUrl}";
                options.UserInteraction.LogoutUrl = $"{origins.IdPClient}/{userInteraction.LogoutUrl}";
                options.UserInteraction.ErrorUrl = $"{origins.IdPClient}/{userInteraction.ErrorUrl}";
            });
            
            var connectionString = configuration.GetConnectionString("Default");
            var migrationsAssembly = "Gaia.IdP.Data";
            
            builder
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, sql =>
                            sql.MigrationsAssembly(migrationsAssembly)
                        );
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, sql =>
                            sql.MigrationsAssembly(migrationsAssembly)
                        );
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<AradUser>();

            if (environment.IsDevelopment())
                builder.AddDeveloperSigningCredential();
            else
                builder.AddDeveloperSigningCredential();
                
            // builder.AddSigningCredential(configService.GetSetting("Certificate.Thumbprint"), StoreLocation.LocalMachine, NameType.Thumbprint);

            return services;
        }
    }
}
