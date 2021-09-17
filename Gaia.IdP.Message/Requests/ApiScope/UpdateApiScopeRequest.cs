using Gaia.IdP.Message.Commands;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class UpdateApiScopeRequest : UpdateApiScopeCommand, IRequest
    {
        public int Id { get; set; }
    }
}
