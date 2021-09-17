using Gaia.IdP.Infrastructure.Enums;
using Gaia.IdP.Infrastructure.Exceptions;
using Gaia.IdP.Infrastructure.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class DomainExceptionHandler
    {
        public static IApplicationBuilder UseDomainExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DomainExceptionHandlerMiddleware>();
        }
    }

    public class DomainExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public DomainExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Serilog.Log.ForContext<DomainExceptionHandlerMiddleware>();;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/problem+json; charset=utf-8";
                DomainResultBase result = null;
                
                if (ex is DomainBadRequestException || ex is DomainException || ex is DbUpdateException)
                {
                    if (ex is DomainBadRequestException domainBadRequestException)
                    {
                        if (domainBadRequestException.ActionExecutingContext != null)
                            result = new DomainBadRequestResult(domainBadRequestException.ActionExecutingContext);
                        else
                            result = new DomainBadRequestResult(domainBadRequestException, context);
                    }
                    else if (ex is DomainException domainException)
                    {
                        result = new DomainClientErrorResult(domainException, context);
                    }
                    else if (ex is DbUpdateException dbUpdateException && dbUpdateException.InnerException.Message.Contains("duplicate key"))
                    {
                        result = new DomainBadRequestResult("database", ErrorMessage.entityWithTheSameKeyAlreadyExists.ToString(), context);
                    }
                    
                    _logger.Error("A handled error accured. {@ErrorDetails}", result);
                }
                else
                {
                    result = new DomainServerErrorResult(ex, context);
                    _logger.Error(ex, "An unhandled exception accured. {@ErrorDetails}", result);
                }
                
                context.Response.StatusCode = result.Status;
                
                var serializedResult = Serialize(result);
                await context.Response.WriteAsync(serializedResult);
            }
        }

        private string Serialize(dynamic err) {
            return JsonConvert.SerializeObject(err, new JsonSerializerSettings {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                MaxDepth = 10
            }); 
        }
    }
}
