using Gaia.IdP.Message.Commands;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class UpdateApiResourceRequest : UpdateApiResourceCommand, IRequest
    {
        public int Id { get; set; }
    }
}
