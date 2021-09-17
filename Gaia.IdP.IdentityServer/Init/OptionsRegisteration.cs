using System;
using System.Linq;
using Gaia.IdP.IdentityServer.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class OptionsRegisteration
    {
        public static IServiceCollection AddRegisterableOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var type = typeof(IRegisterableOptions);

            var optionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && t.IsClass && type.IsAssignableFrom(t));

            foreach (var optionType in optionTypes)
            {
                var option = (OptionsBase)Activator.CreateInstance(optionType, configuration);
                
                services.AddSingleton(optionType, option);
            }

            return services;
        }
    }
}
