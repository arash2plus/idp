using System.Threading.Tasks;
using Gaia.IdP.IdentityServer.Extensions;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using Gaia.IdP.Message.Responses;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static IdentityServer4.IdentityServerConstants;
using System.Collections.Generic;
using Gaia.IdP.Message.Filters;

namespace Gaia.IdP.IdentityServer.Controllers
{
    [Route("api/profile")]
    public class ProfileController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ProfileController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// get current user's pofile
        /// </summary>
        [Authorize(LocalApi.PolicyName)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserProfile>> Get()
        {
            var userId = Request.GetUserId();
            var request = new GetUserProfileRequest { UserId = userId };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary> 
        /// update current user's pofile
        ///  </summary>
        /// <param name="command"></param>
        [Authorize(LocalApi.PolicyName)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserProfile>> Update([FromBody] UpdateUserProfileCommand command)
        {
            var userId = Request.GetUserId();
            var request = _mapper.Map<UpdateUserProfileRequest>(command);
            request.UserId = userId;
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}