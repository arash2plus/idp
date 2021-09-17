using IdentityServer4.Models;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetClientRequest : IRequest<Client>
    {
        public int Id { get; set; }
    }
}
