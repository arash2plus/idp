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
    public class ApiScopeRequestsHandler : 
        IRequestHandler<GetApiScopesCountRequest, int>,
        IRequestHandler<GetApiScopesRequest, IEnumerable<ApiScopeListItem>>,
        IRequestHandler<GetApiScopeRequest, IS4Models.ApiScope>,
        IRequestHandler<CreateApiScopeRequest>,
        IRequestHandler<UpdateApiScopeRequest>,
        IRequestHandler<DeleteApiScopeRequest>
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ApiScopeRequestsHandler(ConfigurationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        
        public Task<int> Handle(GetApiScopesCountRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IS4Entities.ApiScope>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.Name))
                query = query.Where(o => o.Name.Contains(request.Filter.Name));
            
            var result = query.Count();

            return Task.FromResult(result);
        }

        public Task<IEnumerable<ApiScopeListItem>> Handle(GetApiScopesRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IS4Entities.ApiScope>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.Name))
                query = query.Where(o => o.Name.Contains(request.Filter.Name));

            if (request.Filter.Skip.HasValue)
                query.Skip(request.Filter.Skip.Value);

            if (request.Filter.Limit.HasValue)
                query.Take(request.Filter.Limit.Value);

            var result = query
                .Select(o => _mapper.Map<ApiScopeListItem>(o))
                .AsEnumerable();
            
            return Task.FromResult(result);
        }

        public Task<IS4Models.ApiScope> Handle(GetApiScopeRequest request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.Set<IS4Entities.ApiScope>()
                .Include(o => o.UserClaims)
                .Include(o => o.Properties)
                .SingleOrDefault(o => o.Id == request.Id);
                
            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.apiScopeNotFound);

            var result = entity.ToModel();
            
            return Task.FromResult(result);
        }

        public Task<Unit> Handle(CreateApiScopeRequest request, CancellationToken cancellationToken)
        {
            var entity = request.ToEntity();

            _dbContext.Set<IS4Entities.ApiScope>().Add(entity);
            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateApiScopeRequest request, CancellationToken cancellationToken)
        {
            var currentEntity = request.ToEntity();
            var entity = _dbContext.Set<IS4Entities.ApiScope>()
                .Include(o => o.UserClaims)
                .Include(o => o.Properties)
                .SingleOrDefault(o => o.Id == request.Id);

            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.apiScopeNotFound);

            var existingUserClaims = entity.UserClaims.ToArray();
            var existingProperties = entity.Properties.ToArray();

            _mapper.Map(currentEntity, entity);

            _dbContext.Set<IS4Entities.ApiScope>().Update(entity);

            _dbContext.Set<IS4Entities.ApiScopeClaim>().RemoveRange(existingUserClaims);
            _dbContext.Set<IS4Entities.ApiScopeClaim>().AddRange(entity.UserClaims);

            _dbContext.Set<IS4Entities.ApiScopeProperty>().RemoveRange(existingProperties);
            _dbContext.Set<IS4Entities.ApiScopeProperty>().AddRange(entity.Properties);

            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(DeleteApiScopeRequest request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.Set<IS4Entities.ApiScope>().SingleOrDefault(o => o.Id == request.Id);
            if (entity == null)
                throw new DomainException(ErrorStatusCode.notFound, ErrorMessage.apiScopeNotFound);

            _dbContext.Set<IS4Entities.ApiScope>().Remove(entity);
            _dbContext.SaveChanges();

            return Task.FromResult(Unit.Value);
        }
    }
}
