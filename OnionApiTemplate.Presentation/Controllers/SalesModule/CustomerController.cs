using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.Customer;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Delete;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Update;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetAll;
using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Api.Controllers.SalesModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController(ISender mediator) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(300)]
        public async Task<IActionResult> GetAll([FromQuery] CustomerQueryParameters queryParameters)
        {
            var result = await mediator.Send(new GetAllCustomersQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(600)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await mediator.Send(new GetCustomerByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Customer")]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            var result = await mediator.Send(new CreateCustomerCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Customer")]
        [CacheInvalidate("/api/SalesInvoice")]
        [CacheInvalidate("/api/SalesOrder")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto dto)
        {
            var result = await mediator.Send(new UpdateCustomerCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("toggle/{id:guid}")]
        [CacheInvalidate("/api/Customer")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            await mediator.Send(new ToggleCustomerCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}