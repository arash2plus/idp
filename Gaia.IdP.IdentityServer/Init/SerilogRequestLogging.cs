using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gaia.IdP.Infrastructure.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Gaia.Jarchi.Publisher.Init
{
    public static class SerilogRequestLogging
    {
        public static void UseCustomizedSerilogRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = async (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestScheme", GetRequestScheme(httpContext));
                    diagnosticContext.Set("RequestHost", GetRequestHost(httpContext));
                    diagnosticContext.Set("RequestBody", await GetRequestBodyAsync(httpContext));
                    // diagnosticContext.Set("RequestForm", await GetRequestFormAsync(httpContext));
                };
            });
        }

        private static string GetRequestScheme(HttpContext httpContext)
        {
            try
            {
                return httpContext.Request.Scheme;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string GetRequestHost(HttpContext httpContext)
        {
            try
            {
                return httpContext.Request.Host.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static async Task<string> GetRequestBodyAsync(HttpContext httpContext)
        {
            try
            {
                var reader = new StreamReader(httpContext.Request.Body);
                var result = await reader.ReadToEndAsync();
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static async Task<string> GetRequestFormAsync(HttpContext httpContext)
        {
            try
            {
                var form = await httpContext.Request.ReadFormAsync();
                var result = form?.Select(o => $"{o.Key} = {o.Value}").GetString() ?? "";
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
