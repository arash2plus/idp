using Gaia.IdP.Message.Commands;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class UpdateClientRequest : UpdateClientCommand, IRequest
    {
        public int Id { get; set; }
    }
}
