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
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.SalesModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalesOrdersController(ISender mediator) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
           ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(60)]
        public async Task<IActionResult> GetSalesOrders([FromQuery] SalesOrdersQueryParameters queryParameters)
        {
            var result = await mediator.Send(new GetAllSalesOrdersQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<IActionResult> GetSalesOrder(Guid id)
        {
            var result = await mediator.Send(new GetSalesOrderQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/SalesOrders")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> CreateSalesOrder([FromBody] CreateSalesOrderDto dto)
        {
            var result = await mediator.Send(new CreateSalesOrderCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetSalesOrder), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/SalesOrders")]
        public async Task<IActionResult> UpdateSalesOrder(Guid id, [FromBody] UpdateSalesOrderDto dto)
        {
            var result = await mediator.Send(new UpdateSalesOrderCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPost("{id:guid}/confirm")]
        [CacheInvalidate("/api/SalesOrders")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> ConfirmOrder(Guid id, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            var rowVersionBytes = Convert.FromBase64String(rowVersion);
            var result = await mediator.Send(new ConfirmOrderCommand(id, rowVersionBytes, CurrentUserId));
            return Ok(result);
        }

        [HttpPost("{id:guid}/ship")]
        [CacheInvalidate("/api/SalesOrders")]
        [CacheInvalidate("/api/Product")]
        [CacheInvalidate("/api/Warehouse")]
        public async Task<IActionResult> ShipOrder(Guid id, [FromBody] ShipOrderDto dto, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            var rowVersionBytes = Convert.FromBase64String(rowVersion);
            var result = await mediator.Send(new ShipOrderCommand(id, dto, rowVersionBytes, CurrentUserId));
            return Ok(result);
        }

        [HttpPost("{id:guid}/cancel")]
        [CacheInvalidate("/api/SalesOrders")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            var rowVersionBytes = Convert.FromBase64String(rowVersion);
            var result = await mediator.Send(new CancelOrderCommand(id, rowVersionBytes, CurrentUserId));
            return Ok(result);
        }

        [HttpPost("{id:guid}/delivered")]
        [CacheInvalidate("/api/SalesOrders")]
        [CacheInvalidate("/api/SalesInvoices")]
        public async Task<IActionResult> DeliverOrder(Guid id, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            var rowVersionBytes = Convert.FromBase64String(rowVersion);
            var result = await mediator.Send(new DeliverOrderCommand(id, rowVersionBytes, CurrentUserId));
            return Ok(result);
        }
    }
}