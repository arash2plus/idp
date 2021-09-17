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
    public class IdentityResourceRequestsHandler : 
        IRequestHandler<GetIdentityResourcesCountRequest, int>,
        IRequestHandler<GetIdentityResourcesRequest, IEnumerable<IdentityResourceListItem>>,
        IRequestHandler<GetIdentityResourceRequest, IS4Models.IdentityResource>,
        IRequestHandler<CreateIdentityResourceRequest>,
        IRequestHandler<UpdateIdentityResourceRequest>,
        IRequestHandler<DeleteIdentityResourceRequest>
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IMapper _mapper;

        public IdentityResourceRequestsHandler(ConfigurationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        
        public Task<int> Handle(GetIdentityResourcesCountRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IS4Entities.IdentityResource>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.Name))
                query = query.Where(o => o.Name.Contains(request.Filter.Name));
            
            var result = query.Count();

            return Task.FromResult(result);
        }

        public Task<IEnumerable<IdentityResourceListItem>> Handle(GetIdentityResourcesRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IS4Entities.IdentityResource>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.Name))
                query = query.Where(o => o.Name.Contains(request.Filter.Name));

            if (request.Filter.Skip.HasValue)
                query.Skip(request.Filter.Skip.Value);

            if (request.Filter.Limit.HasValue)
                query.Take(request.Filter.Limit.Value);

            var result = query
                .Select(o => _mapper.Map<IdentityResourceListItem>(o))
                .AsEnumerable();
            
            return Task.FromResult(result);
        }

        public Task<IS4Models.IdentityResource> Handle(GetIdentityResourceRequest request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.Set<IS4Entities.IdentityResource>()
                .Include(o => o.UserClaims)
                .Include(o => o.Properties)
                .SingleOrDefault(o => o.Id == request.Id);

            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.identityResourceNotFound);

            var result = entity.ToModel();
            
            return Task.FromResult(result);
        }

        public Task<Unit> Handle(CreateIdentityResourceRequest request, CancellationToken cancellationToken)
        {
            var entity = request.ToEntity();

            _dbContext.Set<IS4Entities.IdentityResource>().Add(entity);
            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateIdentityResourceRequest request, CancellationToken cancellationToken)
        {
            var currentEntity = request.ToEntity();
            var entity = _dbContext.Set<IS4Entities.IdentityResource>()
                .Include(o => o.UserClaims)
                .Include(o => o.Properties)
                .SingleOrDefault(o => o.Id == request.Id);

            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.identityResourceNotFound);

            var existingUserClaims = entity.UserClaims.ToArray();
            var existingProperties = entity.Properties.ToArray();

            _mapper.Map(currentEntity, entity);

            _dbContext.Set<IS4Entities.IdentityResource>().Update(entity);

            _dbContext.Set<IS4Entities.IdentityResourceClaim>().RemoveRange(existingUserClaims);
            _dbContext.Set<IS4Entities.IdentityResourceClaim>().AddRange(entity.UserClaims);

            _dbContext.Set<IS4Entities.IdentityResourceProperty>().RemoveRange(existingProperties);
            _dbContext.Set<IS4Entities.IdentityResourceProperty>().AddRange(entity.Properties);

            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(DeleteIdentityResourceRequest request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.Set<IS4Entities.IdentityResource>().SingleOrDefault(o => o.Id == request.Id);
            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.identityResourceNotFound);

            _dbContext.Set<IS4Entities.IdentityResource>().Remove(entity);
            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
