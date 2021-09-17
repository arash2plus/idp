using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using System;
using Gaia.IdP.Infrastructure.Exceptions;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class Mvc
    {
        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services)
        {
            services.AddMvc()
                .AddCustomizedMvcOptions()
                .ConfigureCustomizedApiBehaviorOptions();

            return services;
        }

        public static IMvcBuilder ConfigureCustomizedApiBehaviorOptions(this IMvcBuilder builder, Action<ApiBehaviorOptions> setupAction = null)
        {
            if (setupAction != null)
                builder.ConfigureApiBehaviorOptions(setupAction);

            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return builder;
        }

        public static IMvcBuilder AddCustomizedMvcOptions(this IMvcBuilder builder, Action<MvcOptions> setupAction = null)
        {
            if (setupAction != null)
                builder.AddMvcOptions(setupAction);

            builder.AddMvcOptions(options =>
            {
                options.Filters.Add(new ModelStateValidationActionFilter());
            });

            return builder;
        }
    }
    
    public class ModelStateValidationActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                throw new DomainBadRequestException(context);
            }
            else
            {
                await next();
            }
        }
    }
}
