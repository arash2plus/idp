using Gaia.IdP.IdentityServer.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class Authentication
    {
        public static IServiceCollection AddCustomizedAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = new OriginsOptions(configuration);            
            
            services
                .AddLocalApiAuthentication();
                // .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                // .AddIdentityServerAuthentication(
                //     JwtBearerDefaults.AuthenticationScheme, 
                //     configureOptions => {
                //         configureOptions.Authority = origins.IdP;
                //         configureOptions.RequireHttpsMetadata = false;
                //         configureOptions.TokenValidationParameters.ValidIssuer = origins.IdP;
                //         configureOptions.Audience = "IdP";
                //     }, o => { }
                // );

            return services;
        }
    }
}
