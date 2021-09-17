using System;
using System.Collections.Generic;
using Gaia.IdP.Message.Filters;
using Gaia.IdP.Message.Responses;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetUserActivityLogsRequest : IRequest<IEnumerable<ActivityLog>>
    {
        public string UserId { get; set; }
        public GetUserActivityLogsPagableFilter Filter { get; set; }
    }
}