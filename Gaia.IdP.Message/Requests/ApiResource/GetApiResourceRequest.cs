using IdentityServer4.Models;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetApiResourceRequest : IRequest<ApiResource>
    {
        public int Id { get; set; }
    }
}
