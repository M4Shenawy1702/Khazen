using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Cancel;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Confirm;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Deliverd;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Ship;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Update;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Queries.GetAll;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.SalesModule
{
    [ApiController]
    [Route("api/salesorders")]
    public class SalesOrdersController(IMediator mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetSalesOrders([FromQuery] SalesOrdersQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllSalesOrdersQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrder(Guid id)
        {
            var query = new GetSalesOrderQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalesOrder([FromBody] CreateSalesOrderDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new CreateSalesOrderCommand(dto, user));
            return CreatedAtAction(nameof(GetSalesOrder), new { id = result.Id }, result);
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateSalesOrder(Guid id, [FromBody] UpdateSalesOrderDto dto, [FromHeader] byte[] rowVersion)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new UpdateSalesOrderCommand(id, dto, user, rowVersion));
            return Ok(result);
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmOrder(Guid id, [FromHeader] byte[] rowVersion)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new ConfirmOrderCommand(id, rowVersion, user));
            return Ok(result);
        }

        [HttpPost("{id}/ship")]
        public async Task<IActionResult> ShipOrder(Guid id, [FromBody] ShipOrderDto dto, [FromHeader] byte[] rowVersion)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new ShipOrderCommand(id, dto, rowVersion, user));
            return Ok(result);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromHeader] byte[] rowVersion)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new CancelOrderCommand(id, rowVersion, user));
            return Ok(result);
        }

        [HttpPost("{id}/delivered")]
        public async Task<IActionResult> DeliverOrder(Guid id, [FromHeader] byte[] rowVersion)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new DeliverOrderCommand(id, rowVersion, user));
            return Ok(result);
        }
    }
}
