using Gaia.IdP.Message.Filters;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetActivityLogsCountRequest : IRequest<int>
    {
        public GetActivityLogsFilter Filter { get; set; }
    }
}
