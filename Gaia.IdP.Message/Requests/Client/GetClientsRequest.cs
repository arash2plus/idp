using System.Collections.Generic;
using Gaia.IdP.Message.Filters;
using Gaia.IdP.Message.Responses;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetClientsRequest : IRequest<IEnumerable<ClientListItem>>
    {
        public GetClientsPagableFilter Filter { get; set; }
    }
}
