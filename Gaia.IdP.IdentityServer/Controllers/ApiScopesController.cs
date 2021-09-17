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
using IdentityServer4.Models;
using Gaia.IdP.Message.Filters;

namespace Gaia.IdP.IdentityServer.Controllers
{
    [Route("api/api-scopes")]
    public class ApiScopesController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ApiScopesController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// get api scopes count by filter
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet("count")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetCount([FromQuery] GetApiScopesFilter filter)
        {
            var request = new GetApiScopesCountRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// get api scopes by filter
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApiScopeListItem>>> GetAll([FromQuery] GetApiScopesPagableFilter filter)
        {
            var request = new GetApiScopesRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// get api scope by id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiScope>> GetById([FromRoute] int id)
        {
            var request = new GetApiScopeRequest { Id = id };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// create api scope
        /// </summary>
        [HttpPost]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Create([FromBody] CreateApiScopeCommand command)
        {
            var request = _mapper.Map<CreateApiScopeRequest>(command);
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// update api scope
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        [HttpPut("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateApiScopeCommand command)
        {
            var request = _mapper.Map<UpdateApiScopeRequest>(command);
            request.Id = id;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// delete api scope
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var request = new DeleteApiScopeRequest { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }
    }
}