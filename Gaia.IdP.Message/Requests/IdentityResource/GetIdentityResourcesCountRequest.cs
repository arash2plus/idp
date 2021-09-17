using Gaia.IdP.Message.Filters;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetIdentityResourcesCountRequest : IRequest<int>
    {
        public GetIdentityResourcesFilter Filter { get; set; }
    }
}
