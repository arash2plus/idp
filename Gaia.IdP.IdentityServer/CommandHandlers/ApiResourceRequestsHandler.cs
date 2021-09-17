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
    public class ApiResourceRequestsHandler : 
        IRequestHandler<GetApiResourcesCountRequest, int>,
        IRequestHandler<GetApiResourcesRequest, IEnumerable<ApiResourceListItem>>,
        IRequestHandler<GetApiResourceRequest, IS4Models.ApiResource>,
        IRequestHandler<CreateApiResourceRequest>,
        IRequestHandler<UpdateApiResourceRequest>,
        IRequestHandler<DeleteApiResourceRequest>
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ApiResourceRequestsHandler(ConfigurationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        
        public Task<int> Handle(GetApiResourcesCountRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IS4Entities.ApiResource>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.Name))
                query = query.Where(o => o.Name.Contains(request.Filter.Name));
            
            var result = query.Count();

            return Task.FromResult(result);
        }

        public Task<IEnumerable<ApiResourceListItem>> Handle(GetApiResourcesRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IS4Entities.ApiResource>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.Name))
                query = query.Where(o => o.Name.Contains(request.Filter.Name));

            if (request.Filter.Skip.HasValue)
                query.Skip(request.Filter.Skip.Value);

            if (request.Filter.Limit.HasValue)
                query.Take(request.Filter.Limit.Value);

            var result = query
                .Select(o => _mapper.Map<ApiResourceListItem>(o))
                .AsEnumerable();
            
            return Task.FromResult(result);
        }

        public Task<IS4Models.ApiResource> Handle(GetApiResourceRequest request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.Set<IS4Entities.ApiResource>()
                .Include(o => o.Scopes)
                .Include(o => o.UserClaims)
                .Include(o => o.Properties)
                .SingleOrDefault(o => o.Id == request.Id);

            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.apiResourceNotFound);

            var result = entity.ToModel();
            
            return Task.FromResult(result);
        }

        public Task<Unit> Handle(CreateApiResourceRequest request, CancellationToken cancellationToken)
        {
            var entity = request.ToEntity();
            entity.Secrets = entity.Secrets
                .Select(o => {
                    var sercret = new IS4Models.Secret(IdentityModel.StringExtensions.ToSha256(o.Value));
                    o.Value = sercret.Value;
                    return o;
                }).ToList();

            _dbContext.Set<IS4Entities.ApiResource>().Add(entity);
            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateApiResourceRequest request, CancellationToken cancellationToken)
        {
            var currentEntity = request.ToEntity();
            var entity = _dbContext.Set<IS4Entities.ApiResource>()
                .Include(o => o.Scopes)
                .Include(o => o.UserClaims)
                .Include(o => o.Properties)
                .Include(o => o.Secrets)
                .SingleOrDefault(o => o.Id == request.Id);

            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.apiResourceNotFound);

            var existingUserClaims = entity.UserClaims.ToArray();
            var existingProperties = entity.Properties.ToArray();
            var existingScopes = entity.Scopes.ToArray();
            var existingSecrets = entity.Secrets.ToArray();

            _mapper.Map(currentEntity, entity);

            _dbContext.Set<IS4Entities.ApiResource>().Update(entity);

            _dbContext.Set<IS4Entities.ApiResourceClaim>().RemoveRange(existingUserClaims);
            _dbContext.Set<IS4Entities.ApiResourceClaim>().AddRange(entity.UserClaims);

            _dbContext.Set<IS4Entities.ApiResourceProperty>().RemoveRange(existingProperties);
            _dbContext.Set<IS4Entities.ApiResourceProperty>().AddRange(entity.Properties);

            _dbContext.Set<IS4Entities.ApiResourceScope>().RemoveRange(existingScopes);
            _dbContext.Set<IS4Entities.ApiResourceScope>().AddRange(entity.Scopes);

            entity.Secrets = entity.Secrets
                .Select(o => {
                    var sercret = new IS4Models.Secret(IdentityModel.StringExtensions.ToSha256(o.Value));
                    o.Value = sercret.Value;
                    return o;
                }).ToList();

            _dbContext.Set<IS4Entities.ApiResourceSecret>().RemoveRange(existingSecrets);
            _dbContext.Set<IS4Entities.ApiResourceSecret>().AddRange(entity.Secrets);

            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(DeleteApiResourceRequest request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.Set<IS4Entities.ApiResource>().SingleOrDefault(o => o.Id == request.Id);
            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.apiResourceNotFound);

            _dbContext.Set<IS4Entities.ApiResource>().Remove(entity);
            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
