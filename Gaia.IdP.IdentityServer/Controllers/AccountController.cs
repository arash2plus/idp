using System.Net;
using System.Threading.Tasks;
using Gaia.IdP.IdentityServer.Extensions;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using Gaia.IdP.Message.Responses;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using static IdentityServer4.IdentityServerConstants;
using System;
using System.Collections.Generic;
using Gaia.IdP.Message.Filters;

namespace Gaia.IdP.IdentityServer.Controllers
{
    [Route("api/account")]
    public class AccountController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AccountController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// register an account
        /// </summary>
        /// <param name="command"></param>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<RegisterAccountResponse>> Register(RegisterAccountCommand command)
        {
            var request = _mapper.Map<RegisterAccountRequest>(command);
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary> 
        /// pre-check login to account via password
        ///  </summary>
        /// <param name="command"></param>
        [HttpPost("login/pre-check")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> LoginPreCheck([FromForm] LoginCommand command)
        {
            var request = _mapper.Map<LoginPreCheckRequest>(command);
            request.HttpRequest = Request;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary> 
        /// login to account via password
        ///  </summary>
        /// <param name="command"></param>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status308PermanentRedirect)]
        public async Task<IActionResult> Login([FromForm] LoginCommand command)
        {
            var request = _mapper.Map<LoginRequest>(command);
            request.HttpRequest = Request;
            await _mediator.Send(request);
            var redirectUrl = WebUtility.UrlDecode(command.ReturnUrl);
            return Redirect(redirectUrl);
        }

        /// <summary> 
        /// pre-check login to account via otp code
        ///  </summary>
        /// <param name="command"></param>
        [HttpPost("login-via-otp/pre-check")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> LoginViaOtpPreCheck([FromBody] LoginViaOtpCommand command)
        {
            var request = _mapper.Map<LoginViaOtpPreCheckRequest>(command);
            request.HttpRequest = Request;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary> 
        /// login to account via otp code
        ///  </summary>
        /// <param name="command"></param>
        [HttpPost("login-via-otp")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status308PermanentRedirect)]
        public async Task<IActionResult> LoginViaOtp([FromBody] LoginViaOtpCommand command)
        {
            var request = _mapper.Map<LoginViaOtpRequest>(command);
            request.HttpRequest = Request;
            var result = await _mediator.Send(request);
            var redirectUrl = WebUtility.UrlDecode(command.ReturnUrl);
            return Redirect(redirectUrl);
        }

        /// <summary>
        /// logout
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LogoutResponse>> Logout([FromForm, Required] string logoutId)
        {
            var request = new LogoutRequest { LogoutId = logoutId, User = User };
            request.HttpRequest = Request;
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        
        /// <summary>
        /// confirm phone number
        /// </summary>
        /// <param name="command"></param>
        [Authorize(LocalApi.PolicyName)]
        [HttpPost("confirm-phone-number")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ConfirmPhoneNumber([FromBody] ConfirmPhoneNumberCommand command)
        {
            var userId = Request.GetUserId();
            var request = _mapper.Map<ConfirmPhoneNumberRequest>(command);
            request.UserId = userId;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// reset password via token received in sms
        /// </summary>
        /// <param name="command"></param>
        [HttpPost("reset-password-via-token-in-sms")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ResetPasswordViaTokenInSms([FromBody] ResetPasswordViaTokenInSmsCommand command)
        {
            var request = _mapper.Map<ResetPasswordViaTokenInSmsRequest>(command);
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// recover password via token received in email
        /// </summary>
        /// <param name="command"></param>
        [HttpPost("recover-password-via-token-in-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RecoverPasswordViaTokenInEmail([FromBody] ResetPasswordViaTokenInEmailCommand command)
        {
            var request = _mapper.Map<ResetPasswordViaTokenInEmailRequest>(command);
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        
        /// <summary> 
        /// get current user's login/logout activity logs count
        ///  </summary>
        /// <param name="filter"></param>
        [Authorize(LocalApi.PolicyName)]
        [HttpGet("activity-logs/count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> GetUserActivityLogsCount([FromQuery] GetUserActivityLogsFilter filter)
        {
            var userId = Request.GetUserId();
            var request = new GetUserActivityLogsCountRequest { UserId = userId, Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        
        /// <summary> 
        /// get current user's login/logout activity logs in a pagable way
        ///  </summary>
        /// <param name="filter"></param>
        [Authorize(LocalApi.PolicyName)]
        [HttpGet("activity-logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetUserActivityLogs([FromQuery] GetUserActivityLogsPagableFilter filter)
        {
            var userId = Request.GetUserId();
            var request = new GetUserActivityLogsRequest { UserId = userId, Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}