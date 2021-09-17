using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using AutoMapper;
using Gaia.IdP.IdentityServer.Init;
using Gaia.IdP.Data.Models;
using IdentityServer4.Services;
using Gaia.IdP.IdentityServer.Services;
using Gaia.Jarchi.Publisher.Init;
using Microsoft.IdentityModel.Logging;

namespace Gaia.IdP.IdentityServer
{
    public class Startup
    {
        private IConfiguration Configuration;
        private IWebHostEnvironment Environment;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomizedDbContext(Configuration);

            services.AddCustomizedIdentity();

            services.AddCustomizedIdentityServer(Configuration, Environment);

            services.AddCustomizedAuthentication(Configuration);

            services.AddCustomizedHttpsRedirectionAndHsts(Environment);

            services.AddCustomizedCors(Configuration, Environment);

            services.AddCustomizedMediatR();

            services.AddCustomizedMvc();

            services.ConfigureNonBreakingSameSiteCookies();

            services.AddAutoMapper();

            services.AddSwagger(Configuration);

            services.AddRegisterableOptions(Configuration);
            
            services.AddEventBusKafka(Configuration);

            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IEventSink, IdentityServerEventsHandler>();
            services.AddTransient<GoogleRecaptchaVerificationService>();

        }

        public void Configure(IApplicationBuilder app)
        {
            IdentityModelEventSource.ShowPII = true;

            app.UseCustomizedSerilogRequestLogging();
            app.UseDomainExceptionHandler();

            app.UseSwaggerAndSwaggerUI(Configuration);

            app.UseIdentityServer();

            app.UseCustomizedHttpsRedirectionAndHsts(Environment);
             
            app.UseRouting();
            
            app.UseCors();

            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.MigrateDbContext<AradDbContext>();
            app.MigrateDbContext<PersistedGrantDbContext>();
            app.MigrateDbContext<ConfigurationDbContext>();

            app.SeedConfigurationDbContext();
        }
    }
}
