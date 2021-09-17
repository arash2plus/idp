using Gaia.IdP.IdentityServer.CommandHandlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class MediatR
    {
        public static IServiceCollection AddCustomizedMediatR(this IServiceCollection services)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            
            services.AddMediatR(assembly);

            return services;
        }
    }
}
