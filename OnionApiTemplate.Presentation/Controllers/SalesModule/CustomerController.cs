using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.Customer;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Delete;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Update;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetAll;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Api.Controllers.SalesModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CustomerQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllCustomersQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(id));
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var command = new CreateCustomerCommand(dto, user);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto Dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var command = new UpdateCustomerCommand(id, Dto, user);

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new ToggleCustomerCommand(id, user));
            return Ok(result);
        }
    }
}
