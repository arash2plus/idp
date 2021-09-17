using Gaia.IdP.Message.Commands;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class ResetPasswordViaTokenInSmsRequest: ResetPasswordViaTokenInSmsCommand, IRequest
    {
    }
}
