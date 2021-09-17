using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Gaia.IdP.IdentityServer.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class Swagger
    {
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerDoc = new SwaggerDocOptions(configuration);
            var origins = new OriginsOptions(configuration);            

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(swaggerDoc.Version, new OpenApiInfo {
                    Title = swaggerDoc.Title,
                    Description = swaggerDoc.Description,
                    Version = swaggerDoc.Version,
                    Contact = new OpenApiContact {
                        Email = swaggerDoc.Contact.Email,
                        Name = swaggerDoc.Contact.Name,
                        Url = new Uri(swaggerDoc.Contact.Url)
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{origins.IdP}/connect/authorize"),
                            TokenUrl = new Uri($"{origins.IdP}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "User identifier" },
                                { "IdentityServerApi", "Dara IdP full access" },
                                { "IdP.Profile", "Your profile info" }
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
                options.CustomSchemaIds(o => o.FullName);
            });
        }

        public class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var hasAuthorize = context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(o => o is AuthorizeAttribute);
                var hasAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(o => o is AllowAnonymousAttribute);

                if (hasAuthorize && !hasAnonymous)
                {
                    var alreadyAdded = operation.Responses.Any(o => o.Key == "401");
                    if (!alreadyAdded) operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });

                    alreadyAdded = operation.Responses.Any(o => o.Key == "403");
                    if (!alreadyAdded) operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement {
                            [
                                new OpenApiSecurityScheme {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "oauth2"
                                    },
                                }
                            ] = new[] { "IdP" }
                        }
                    };
                }
            }
        }

        public static void UseSwaggerAndSwaggerUI(this IApplicationBuilder app, IConfiguration configuration)
        {
            var swaggerDocOptions = new SwaggerDocOptions(configuration);

            app.UseSwagger();

            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint($"/swagger/{swaggerDocOptions.Version}/swagger.json", swaggerDocOptions.Title);
                options.RoutePrefix = "swagger";
                options.DocExpansion(DocExpansion.None);

                options.OAuthClientId("idp_swagger");
                options.OAuthAppName("Dara IdP Swagger UI");
                options.OAuthUsePkce();
            });
        }
    }
}
