using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IS4Entities = IdentityServer4.EntityFramework.Entities;
using IS4Models = IdentityServer4.Models;
using IdentityServer4.EntityFramework.Mappers;
using Gaia.IdP.IdentityServer.Options;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Collections.Generic;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class Seed
    {
        public static void SeedConfigurationDbContext(this IApplicationBuilder app)
        {            
            var configuration = GetSeedConfiguration();

            var seedOptions = new IS4SeedOptions(configuration);
            if (seedOptions.Enabled)
            {
                using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var dbContext = serviceScope.ServiceProvider.GetService<ConfigurationDbContext>();
                var mapper = serviceScope.ServiceProvider.GetService<IMapper>();

                SeedIdentityResources(seedOptions.IdentityResources, dbContext, mapper);
                SeedApiScopes(seedOptions.ApiScopes, dbContext, mapper);
                SeedApiResources(seedOptions.ApiResources, dbContext, mapper);
                SeedClients(seedOptions.Clients, dbContext, mapper);

                dbContext.SaveChanges();
            }
        }

        private static void SeedIdentityResources(IEnumerable<IS4Models.IdentityResource> identityResources, ConfigurationDbContext dbContext, IMapper mapper)
        {
            var currentEntities = identityResources.Select(o => o.ToEntity()).ToArray();
            
            var existingEntityNames = dbContext.Set<IS4Entities.IdentityResource>().Select(o => o.Name).ToArray();
            var entitiesToAdd = currentEntities.Where(o => !existingEntityNames.Any(x => x == o.Name)).ToArray();
            dbContext.Set<IS4Entities.IdentityResource>().AddRange(entitiesToAdd);

            var updatingEntityNames = currentEntities.Except(entitiesToAdd).Select(o => o.Name);
            foreach (var name in updatingEntityNames)
            {
                var currentEntity = currentEntities.Single(o => o.Name == name);
                var entity = dbContext.Set<IS4Entities.IdentityResource>()
                    .Include(o => o.UserClaims)
                    .Include(o => o.Properties)
                    .Single(o => o.Name == name);

                var existingUserClaims = entity.UserClaims.ToArray();
                var existingProperties = entity.Properties.ToArray();

                mapper.Map(currentEntity, entity);

                dbContext.Set<IS4Entities.IdentityResource>().Update(entity);

                dbContext.Set<IS4Entities.IdentityResourceClaim>().RemoveRange(existingUserClaims);
                dbContext.Set<IS4Entities.IdentityResourceClaim>().AddRange(entity.UserClaims);

                dbContext.Set<IS4Entities.IdentityResourceProperty>().RemoveRange(existingProperties);
                dbContext.Set<IS4Entities.IdentityResourceProperty>().AddRange(entity.Properties);
            }
        }

        private static void SeedApiScopes(IEnumerable<IS4Models.ApiScope> apiScopes, ConfigurationDbContext dbContext, IMapper mapper)
        {
            var currentEntities = apiScopes.Select(o => o.ToEntity()).ToArray();

            var existingEntityNames = dbContext.Set<IS4Entities.ApiScope>().Select(o => o.Name).ToArray();
            var entitiesToAdd = currentEntities.Where(o => !existingEntityNames.Any(x => x == o.Name)).ToArray();
            dbContext.Set<IS4Entities.ApiScope>().AddRange(entitiesToAdd);
            
            var updatingEntityNames = currentEntities.Except(entitiesToAdd).Select(o => o.Name);
            foreach (var name in updatingEntityNames)
            {
                var currentEntity = currentEntities.Single(o => o.Name == name);
                var entity = dbContext.Set<IS4Entities.ApiScope>()
                    .Include(o => o.UserClaims)
                    .Include(o => o.Properties)
                    .Single(o => o.Name == name);

                var existingUserClaims = entity.UserClaims.ToArray();
                var existingProperties = entity.Properties.ToArray();

                mapper.Map(currentEntity, entity);

                dbContext.Set<IS4Entities.ApiScope>().Update(entity);

                dbContext.Set<IS4Entities.ApiScopeClaim>().RemoveRange(existingUserClaims);
                dbContext.Set<IS4Entities.ApiScopeClaim>().AddRange(entity.UserClaims);

                dbContext.Set<IS4Entities.ApiScopeProperty>().RemoveRange(existingProperties);
                dbContext.Set<IS4Entities.ApiScopeProperty>().AddRange(entity.Properties);
            }
        }

        private static void SeedApiResources(IEnumerable<IS4Models.ApiResource> apiResources, ConfigurationDbContext dbContext, IMapper mapper)
        {
            var currentEntities = apiResources.Select(o => {
                var entity = o.ToEntity();
                entity.Secrets = entity.Secrets
                    .Select(o => {
                        var sercret = new IS4Models.Secret(IdentityModel.StringExtensions.ToSha256(o.Value));
                        o.Value = sercret.Value;
                        return o;
                    }).ToList();

                return entity;
            }).ToArray();

            var existingEntityNames = dbContext.Set<IS4Entities.ApiResource>().Select(o => o.Name).ToArray();
            var entitiesToAdd = currentEntities.Where(o => !existingEntityNames.Any(x => x == o.Name)).ToArray();
            dbContext.Set<IS4Entities.ApiResource>().AddRange(entitiesToAdd);

            var updatingEntityNames = currentEntities.Except(entitiesToAdd).Select(o => o.Name);
            foreach (var name in updatingEntityNames)
            {
                var currentEntity = currentEntities.Single(o => o.Name == name);
                var entity = dbContext.Set<IS4Entities.ApiResource>()
                    .Include(o => o.UserClaims)
                    .Include(o => o.Properties)
                    .Include(o => o.Scopes)
                    .Include(o => o.Secrets)
                    .Single(o => o.Name == name);

                var existingUserClaims = entity.UserClaims.ToArray();
                var existingProperties = entity.Properties.ToArray();
                var existingScopes = entity.Scopes.ToArray();
                var existingSecrets = entity.Secrets.ToArray();

                mapper.Map(currentEntity, entity);

                dbContext.Set<IS4Entities.ApiResource>().Update(entity);

                dbContext.Set<IS4Entities.ApiResourceClaim>().RemoveRange(existingUserClaims);
                dbContext.Set<IS4Entities.ApiResourceClaim>().AddRange(entity.UserClaims);

                dbContext.Set<IS4Entities.ApiResourceProperty>().RemoveRange(existingProperties);
                dbContext.Set<IS4Entities.ApiResourceProperty>().AddRange(entity.Properties);

                dbContext.Set<IS4Entities.ApiResourceScope>().RemoveRange(existingScopes);
                dbContext.Set<IS4Entities.ApiResourceScope>().AddRange(entity.Scopes);

                entity.Secrets = currentEntity.Secrets
                    .Select(o => {
                        var sercret = new IS4Models.Secret(IdentityModel.StringExtensions.ToSha256(o.Value));
                        o.Value = sercret.Value;
                        return o;
                    }).ToList();

                dbContext.Set<IS4Entities.ApiResourceSecret>().RemoveRange(existingSecrets);
                dbContext.Set<IS4Entities.ApiResourceSecret>().AddRange(entity.Secrets);
            }
        }

        private static void SeedClients(IEnumerable<IS4Models.Client> clients, ConfigurationDbContext dbContext, IMapper mapper)
        {
            var currentEntities = clients.Select(o => {
                var entity = o.ToEntity();
                entity.ClientSecrets = entity.ClientSecrets
                    .Select(o => {
                        var sercret = new IS4Models.Secret(IdentityModel.StringExtensions.ToSha256(o.Value));
                        o.Value = sercret.Value;
                        return o;
                    }).ToList();

                return entity;
            }).ToArray();

            var existingEntityClientIds = dbContext.Set<IS4Entities.Client>().Select(o => o.ClientId).ToArray();
            var entitiesToAdd = currentEntities.Where(o => !existingEntityClientIds.Any(x => x == o.ClientId)).ToArray();
            dbContext.Set<IS4Entities.Client>().AddRange(entitiesToAdd);
            
            var updatingEntityClientIds = currentEntities.Except(entitiesToAdd).Select(o => o.ClientId);
            foreach (var clientId in updatingEntityClientIds)
            {
                var currentEntity = currentEntities.Single(o => o.ClientId == clientId);
                var entity = dbContext.Set<IS4Entities.Client>()
                    .Include(o => o.AllowedGrantTypes)
                    .Include(o => o.AllowedScopes)
                    .Include(o => o.AllowedCorsOrigins)
                    .Include(o => o.ClientSecrets)
                    .Include(o => o.PostLogoutRedirectUris)
                    .Include(o => o.Properties)
                    .Include(o => o.RedirectUris)
                    .Single(o => o.ClientId == clientId);

                var existingAllowedGrantTypes = entity.AllowedGrantTypes.ToArray();
                var existingAllowedScopes = entity.AllowedScopes.ToArray();
                var existingAllowedCorsOrigins = entity.AllowedCorsOrigins.ToArray();
                var existingClientSecrets = entity.ClientSecrets.ToArray();
                var existingPostLogoutRedirectUris = entity.PostLogoutRedirectUris.ToArray();
                var existingProperties = entity.Properties.ToArray();
                var existingRedirectUris = entity.RedirectUris.ToArray();

                mapper.Map(currentEntity, entity);

                dbContext.Set<IS4Entities.Client>().Update(entity);

                dbContext.Set<IS4Entities.ClientGrantType>().RemoveRange(existingAllowedGrantTypes);
                dbContext.Set<IS4Entities.ClientGrantType>().AddRange(entity.AllowedGrantTypes);

                dbContext.Set<IS4Entities.ClientScope>().RemoveRange(existingAllowedScopes);
                dbContext.Set<IS4Entities.ClientScope>().AddRange(entity.AllowedScopes);

                dbContext.Set<IS4Entities.ClientCorsOrigin>().RemoveRange(existingAllowedCorsOrigins);
                dbContext.Set<IS4Entities.ClientCorsOrigin>().AddRange(entity.AllowedCorsOrigins);

                entity.ClientSecrets = entity.ClientSecrets
                    .Select(o => {
                        var sercret = new IS4Models.Secret(IdentityModel.StringExtensions.ToSha256(o.Value));
                        o.Value = sercret.Value;
                        return o;
                    }).ToList();

                dbContext.Set<IS4Entities.ClientSecret>().RemoveRange(existingClientSecrets);
                dbContext.Set<IS4Entities.ClientSecret>().AddRange(entity.ClientSecrets);

                dbContext.Set<IS4Entities.ClientPostLogoutRedirectUri>().RemoveRange(existingPostLogoutRedirectUris);
                dbContext.Set<IS4Entities.ClientPostLogoutRedirectUri>().AddRange(entity.PostLogoutRedirectUris);

                dbContext.Set<IS4Entities.ClientProperty>().RemoveRange(existingProperties);
                dbContext.Set<IS4Entities.ClientProperty>().AddRange(entity.Properties);

                dbContext.Set<IS4Entities.ClientRedirectUri>().RemoveRange(existingRedirectUris);
                dbContext.Set<IS4Entities.ClientRedirectUri>().AddRange(entity.RedirectUris);
            }
        }

        private static IConfiguration GetSeedConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
            
            if (environment == "Development")
                configurationBuilder
                    .AddJsonFile($"appsettings.Development.json", optional: false, reloadOnChange: false);
            else
                configurationBuilder
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            var configuration = configurationBuilder.Build();

            return configuration;
        }
    }
}
