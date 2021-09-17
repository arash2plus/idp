using IdentityServer4.Models;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetApiScopeRequest : IRequest<ApiScope>
    {
        public int Id { get; set; }
    }
}
