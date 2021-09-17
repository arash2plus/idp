using System.Threading.Tasks;
using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Requests;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using IdentityServer4.Models;
using Gaia.IdP.Message.Filters;
using Gaia.IdP.Message.Responses;

namespace Gaia.IdP.IdentityServer.Controllers
{
    [Route("api/identity-resources")]
    public class IdentityResourcesController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public IdentityResourcesController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// get identity resource count by filter
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet("count")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetCount([FromQuery] GetIdentityResourcesFilter filter)
        {
            var request = new GetIdentityResourcesCountRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// get identity resources by filter
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<IdentityResourceListItem>>> GetAll([FromQuery] GetIdentityResourcesPagableFilter filter)
        {
            var request = new GetIdentityResourcesRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// get identity resource by id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IdentityResource>> GetById([FromRoute] int id)
        {
            var request = new GetIdentityResourceRequest { Id = id };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// create identity resource
        /// </summary>
        [HttpPost]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Create([FromBody] CreateIdentityResourceCommand command)
        {
            var request = _mapper.Map<CreateIdentityResourceRequest>(command);
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// update identity resource
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        [HttpPut("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateIdentityResourceCommand command)
        {
            var request = _mapper.Map<UpdateIdentityResourceRequest>(command);
            request.Id = id;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// delete identity resource
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var request = new DeleteIdentityResourceRequest { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }
    }
}