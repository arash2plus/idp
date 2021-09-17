using Gaia.IdP.Message.Filters;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetApiResourcesCountRequest : IRequest<int>
    {
        public GetApiResourcesFilter Filter { get; set; }
    }
}
