using IdentityServer4.Models;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetIdentityResourceRequest : IRequest<IdentityResource>
    {
        public int Id { get; set; }
    }
}
