using System.Collections.Generic;
using Gaia.IdP.Message.Filters;
using Gaia.IdP.Message.Responses;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetApiResourcesRequest : IRequest<IEnumerable<ApiResourceListItem>>
    {
        public GetApiResourcesPagableFilter Filter { get; set; }
    }
}
