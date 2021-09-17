using Gaia.IdP.Message.Filters;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetApiScopesCountRequest : IRequest<int>
    {
        public GetApiScopesFilter Filter { get; set; }
    }
}
