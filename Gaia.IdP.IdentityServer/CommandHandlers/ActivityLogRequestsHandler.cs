using Gaia.IdP.Message.Requests;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using IdPEntities = Gaia.IdP.DomainModel.Models;
using IdPDtos = Gaia.IdP.Message.Responses;
using Microsoft.EntityFrameworkCore;
using Gaia.IdP.Data.Models;

namespace Gaia.IdP.IdentityServer.CommandHandlers
{
    public class ActivityLogRequestsHandler : 
        IRequestHandler<GetActivityLogsCountRequest, int>,
        IRequestHandler<GetActivityLogsRequest, IEnumerable<IdPDtos.ActivityLog>>,
        IRequestHandler<GetUserActivityLogsCountRequest, int>,
        IRequestHandler<GetUserActivityLogsRequest, IEnumerable<IdPDtos.ActivityLog>>
    {
        private readonly AradDbContext _dbContext;
        private readonly IMapper _mapper;

        public ActivityLogRequestsHandler(AradDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task<int> Handle(GetActivityLogsCountRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IdPEntities.ActivityLog>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.UserName))
                query = query.Where(o => o.User.UserName.Contains(request.Filter.UserName));

            if (request.Filter.From.HasValue)
                query = query.Where(o => o.Date >= request.Filter.From);

            if (request.Filter.To.HasValue)
                query = query.Where(o => o.Date <= request.Filter.To);
            
            var result = query.Count();

            return Task.FromResult(result);
        }

        public Task<IEnumerable<IdPDtos.ActivityLog>> Handle(GetActivityLogsRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IdPEntities.ActivityLog>()
                .Include(o => o.User)
                .OrderByDescending(o => o.Date)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter.UserName))
                query = query.Where(o => o.User.UserName.Contains(request.Filter.UserName));

            if (request.Filter.From.HasValue)
                query = query.Where(o => o.Date >= request.Filter.From);

            if (request.Filter.To.HasValue)
                query = query.Where(o => o.Date <= request.Filter.To);

            if (request.Filter.Skip.HasValue)
                query.Skip(request.Filter.Skip.Value);

            if (request.Filter.Limit.HasValue)
                query.Take(request.Filter.Limit.Value);

            var result = query
                .Select(o => _mapper.Map<IdPDtos.ActivityLog>(o))
                .AsEnumerable();
            
            return Task.FromResult(result);            
        }

        public Task<int> Handle(GetUserActivityLogsCountRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IdPEntities.ActivityLog>().Where(o => o.UserId == request.UserId);

            if (request.Filter.From.HasValue)
                query = query.Where(o => o.Date >= request.Filter.From);

            if (request.Filter.To.HasValue)
                query = query.Where(o => o.Date <= request.Filter.To);
            
            var result = query.Count();

            return Task.FromResult(result);
        }

        public Task<IEnumerable<IdPDtos.ActivityLog>> Handle(GetUserActivityLogsRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<IdPEntities.ActivityLog>()
                .Include(o => o.User)
                .OrderByDescending(o => o.Date)
                .Where(o => o.UserId == request.UserId);

            if (request.Filter.From.HasValue)
                query = query.Where(o => o.Date >= request.Filter.From);

            if (request.Filter.To.HasValue)
                query = query.Where(o => o.Date <= request.Filter.To);

            if (request.Filter.Skip.HasValue)
                query.Skip(request.Filter.Skip.Value);

            if (request.Filter.Limit.HasValue)
                query.Take(request.Filter.Limit.Value);

            var result = query
                .Select(o => _mapper.Map<IdPDtos.ActivityLog>(o))
                .AsEnumerable();
            
            return Task.FromResult(result);
        }
    }
}
