using System;
using Gaia.IdP.Message.Filters;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetUserActivityLogsCountRequest : IRequest<int>
    {
        public string UserId { get; set; }
        public GetUserActivityLogsFilter Filter { get; set; }
    }
}
