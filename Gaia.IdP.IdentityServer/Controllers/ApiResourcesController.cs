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
    [Route("api/api-resources")]
    public class ApiResourcesController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ApiResourcesController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// get api resource count by filter
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet("count")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetCount([FromQuery] GetApiResourcesFilter filter)
        {
            var request = new GetApiResourcesCountRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// get api resources by filter
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApiResourceListItem>>> GetAll([FromQuery] GetApiResourcesPagableFilter filter)
        {
            var request = new GetApiResourcesRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// get api resource by id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResource>> GetById([FromRoute] int id)
        {
            var request = new GetApiResourceRequest { Id = id };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// create api resource
        /// </summary>
        [HttpPost]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Create([FromBody] CreateApiResourceCommand command)
        {
            var request = _mapper.Map<CreateApiResourceRequest>(command);
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// update api resource
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        [HttpPut("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateApiResourceCommand command)
        {
            var request = _mapper.Map<UpdateApiResourceRequest>(command);
            request.Id = id;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// delete api resource
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var request = new DeleteApiResourceRequest { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }
    }
}