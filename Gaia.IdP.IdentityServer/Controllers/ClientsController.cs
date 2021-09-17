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
    [Route("api/clients")]
    public class ClientsController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ClientsController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// get clients count by filter
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet("count")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetCount([FromQuery] GetClientsFilter filter)
        {
            var request = new GetClientsCountRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// get clients by filter
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClientListItem>>> GetAll([FromQuery] GetClientsPagableFilter filter)
        {
            var request = new GetClientsRequest { Filter = filter };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// get client by id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Client>> GetById([FromRoute] int id)
        {
            var request = new GetClientRequest { Id = id };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// create client
        /// </summary>
        [HttpPost]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Create([FromBody] CreateClientCommand command)
        {
            var request = _mapper.Map<CreateClientRequest>(command);
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// update client
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        [HttpPut("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateClientCommand command)
        {
            var request = _mapper.Map<UpdateClientRequest>(command);
            request.Id = id;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// delete client
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        // [Authorize(LocalApi.PolicyName)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var request = new DeleteClientRequest { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }
    }
}