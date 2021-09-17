using Gaia.IdP.Message.Filters;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetClientsCountRequest : IRequest<int>
    {
        public GetClientsFilter Filter { get; set; }
    }
}
