using System.Collections.Generic;
using Gaia.IdP.Message.Filters;
using Gaia.IdP.Message.Responses;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetActivityLogsRequest : IRequest<IEnumerable<ActivityLog>>
    {
        public GetActivityLogsPagableFilter Filter { get; set; }
    }
}