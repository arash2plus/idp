using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Gaia.IdP.IdentityServer.Extensions;
using Gaia.IdP.Message.Requests;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gaia.IdP.Message.ValidationAttributes;

namespace Gaia.IdP.IdentityServer.Controllers
{
    [Route("api/account-tokens")]
    public class AccountTokenController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AccountTokenController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary> 
        /// send login token (otp) to provided phone number
        ///  </summary>
        /// <param name="phoneNumber"></param>
        [HttpGet("login")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SendLoginTokenViaSms([FromQuery, PhoneNumberValidation, Required] string phoneNumber)
        {
            var request = new SendLoginTokenViaSmsRequest { PhoneNumber = phoneNumber };
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary> 
        /// send phone number confirmation token to user's registered phone number
        ///  </summary>
        [HttpGet("phone-number-confirmation")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult> SendMobileNumberConfirmationToken()
        {
            var request = new SendPhoneNumberConfirmationTokenRequest { UserId = Request.GetUserId() };
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// send password recovery token to provided phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        [HttpGet("reset-password-via-sms")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SendPasswordRecoveryTokenViaSms([FromQuery, PhoneNumberValidation, Required] string phoneNumber)
        {
            var request = new SendResetPasswordTokenViaSmsRequest { PhoneNumber = phoneNumber };
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// send password recovery token to provided email
        /// </summary>
        /// <param name="email"></param>
        [HttpGet("reset-password-via-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SendPasswordRecoveryTokenViaEmail([FromQuery, EmailAddress, Required] string email)
        {
            var request = new SendResetPasswordTokenViaEmailRequest { Email = email };
            await _mediator.Send(request);
            return NoContent();
        }
    }
}