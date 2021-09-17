using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class AutoMapper
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var types = new [] { typeof(Profiles.AccountProfile) };

            var profiles = types
                .Select(t => t.Assembly)
                .SelectMany(o => o.GetExportedTypes())
                .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                .Where(t => !t.GetTypeInfo().IsAbstract);

            var mapperConfig = new MapperConfiguration(mc =>
            {
                foreach (var profile in profiles)
                {
                    mc.AddProfile(profile);
                }
            });

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);
            return services;
        }
    }
}
