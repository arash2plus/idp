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
    [Route("api/accounts")]
    public class AccountsController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AccountsController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }
        
        /// <summary> 
        /// get users login/logout activity logs count
        ///  </summary>
        /// <param name="filter"></param>
        [Authorize(LocalApi.PolicyName)]
        [HttpGet("activity-logs/count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> GetActivityLogsCount([FromQuery] GetActivityLogsFilter filter)
        {
            var request = new GetActivityLogsCountRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        
        /// <summary> 
        /// get users login/logout activity logs
        ///  </summary>
        /// <param name="filter"></param>
        [Authorize(LocalApi.PolicyName)]
        [HttpGet("activity-logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetActivityLogs([FromQuery] GetActivityLogsPagableFilter filter)
        {
            var request = new GetActivityLogsRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}