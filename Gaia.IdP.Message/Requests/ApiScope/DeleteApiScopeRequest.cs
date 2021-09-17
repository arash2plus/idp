using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class DeleteApiScopeRequest : IRequest
    {
        public int Id { get; set; }
    }
}
