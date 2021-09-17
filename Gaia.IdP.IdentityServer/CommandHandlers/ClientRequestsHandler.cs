using Gaia.IdP.Infrastructure.Enums;
using Gaia.IdP.Infrastructure.Exceptions;
using Gaia.IdP.Message.Requests;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using IS4Entities = IdentityServer4.EntityFramework.Entities;
using IS4Models = IdentityServer4.Models;
using IdentityServer4.EntityFramework.DbContexts;
using System.Linq;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.Mappers;
using Gaia.IdP.Message.Responses;
using Microsoft.EntityFrameworkCore;

namespace Gaia.IdP.IdentityServer.CommandHandlers
{
    public class ClientRequestsHandler : 
        IRequestHandler<GetClientsCountRequest, int>,
        IRequestHandler<GetClientsRequest, IEnumerable<ClientListItem>>,
        IRequestHandler<GetClientRequest, IS4Models.Client>,
        IRequestHandler<CreateClientRequest>,
        IRequestHandler<UpdateClientRequest>,
        IRequestHandler<DeleteClientRequest>
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ClientRequestsHandler(ConfigurationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        
        public Task<int> Handle(GetClientsCountRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IS4Entities.Client>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.ClientId))
                query = query.Where(o => o.ClientId.Contains(request.Filter.ClientId));

            if (!string.IsNullOrEmpty(request.Filter.ClientName))
                query = query.Where(o => o.ClientName.Contains(request.Filter.ClientName));
            
            var result = query.Count();

            return Task.FromResult(result);
        }

        public Task<IEnumerable<ClientListItem>> Handle(GetClientsRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IS4Entities.Client>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.ClientId))
                query = query.Where(o => o.ClientId.Contains(request.Filter.ClientId));

            if (!string.IsNullOrEmpty(request.Filter.ClientName))
                query = query.Where(o => o.ClientName.Contains(request.Filter.ClientName));

            if (request.Filter.Skip.HasValue)
                query.Skip(request.Filter.Skip.Value);

            if (request.Filter.Limit.HasValue)
                query.Take(request.Filter.Limit.Value);

            var result = query
                .Select(o => _mapper.Map<ClientListItem>(o))
                .AsEnumerable();
            
            return Task.FromResult(result);
        }

        public Task<IS4Models.Client> Handle(GetClientRequest request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.Set<IS4Entities.Client>()
                .Include(o => o.AllowedGrantTypes)
                .Include(o => o.AllowedScopes)
                .Include(o => o.AllowedCorsOrigins)
                .Include(o => o.PostLogoutRedirectUris)
                .Include(o => o.Properties)
                .Include(o => o.RedirectUris)
                .SingleOrDefault(o => o.Id == request.Id);

            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.clientNotFound);

            var result = entity.ToModel();
            
            return Task.FromResult(result);
        }

        public Task<Unit> Handle(CreateClientRequest request, CancellationToken cancellationToken)
        {
            var entity = request.ToEntity();
            entity.ClientSecrets = entity.ClientSecrets
                .Select(o => {
                    var sercret = new IS4Models.Secret(IdentityModel.StringExtensions.ToSha256(o.Value));
                    o.Value = sercret.Value;
                    return o;
                }).ToList();

            _dbContext.Set<IS4Entities.Client>().Add(entity);
            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateClientRequest request, CancellationToken cancellationToken)
        {
            var currentEntity = request.ToEntity();
            var entity = _dbContext.Set<IS4Entities.Client>()
                .Include(o => o.AllowedGrantTypes)
                .Include(o => o.AllowedScopes)
                .Include(o => o.AllowedCorsOrigins)
                .Include(o => o.ClientSecrets)
                .Include(o => o.PostLogoutRedirectUris)
                .Include(o => o.Properties)
                .Include(o => o.RedirectUris)
                .SingleOrDefault(o => o.Id == request.Id);

            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.clientNotFound);


            var existingAllowedGrantTypes = entity.AllowedGrantTypes.ToArray();
            var existingAllowedScopes = entity.AllowedScopes.ToArray();
            var existingAllowedCorsOrigins = entity.AllowedCorsOrigins.ToArray();
            var existingClientSecrets = entity.ClientSecrets.ToArray();
            var existingPostLogoutRedirectUris = entity.PostLogoutRedirectUris.ToArray();
            var existingProperties = entity.Properties.ToArray();
            var existingRedirectUris = entity.RedirectUris.ToArray();

            _mapper.Map(currentEntity, entity);

            _dbContext.Set<IS4Entities.Client>().Update(entity);

            _dbContext.Set<IS4Entities.ClientGrantType>().RemoveRange(existingAllowedGrantTypes);
            _dbContext.Set<IS4Entities.ClientGrantType>().AddRange(entity.AllowedGrantTypes);

            _dbContext.Set<IS4Entities.ClientScope>().RemoveRange(existingAllowedScopes);
            _dbContext.Set<IS4Entities.ClientScope>().AddRange(entity.AllowedScopes);

            _dbContext.Set<IS4Entities.ClientCorsOrigin>().RemoveRange(existingAllowedCorsOrigins);
            _dbContext.Set<IS4Entities.ClientCorsOrigin>().AddRange(entity.AllowedCorsOrigins);

            entity.ClientSecrets = entity.ClientSecrets
                .Select(o => {
                    var sercret = new IS4Models.Secret(IdentityModel.StringExtensions.ToSha256(o.Value));
                    o.Value = sercret.Value;
                    return o;
                }).ToList();

            _dbContext.Set<IS4Entities.ClientSecret>().RemoveRange(existingClientSecrets);
            _dbContext.Set<IS4Entities.ClientSecret>().AddRange(entity.ClientSecrets);

            _dbContext.Set<IS4Entities.ClientPostLogoutRedirectUri>().RemoveRange(existingPostLogoutRedirectUris);
            _dbContext.Set<IS4Entities.ClientPostLogoutRedirectUri>().AddRange(entity.PostLogoutRedirectUris);

            _dbContext.Set<IS4Entities.ClientProperty>().RemoveRange(existingProperties);
            _dbContext.Set<IS4Entities.ClientProperty>().AddRange(entity.Properties);

            _dbContext.Set<IS4Entities.ClientRedirectUri>().RemoveRange(existingRedirectUris);
            _dbContext.Set<IS4Entities.ClientRedirectUri>().AddRange(entity.RedirectUris);

            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(DeleteClientRequest request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.Set<IS4Entities.Client>().SingleOrDefault(o => o.Id == request.Id);
            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.clientNotFound);

            _dbContext.Set<IS4Entities.Client>().Remove(entity);
            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
