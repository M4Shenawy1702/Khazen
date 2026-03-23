using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseOrdersController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPost]
        [CacheInvalidate("/api/PurchaseOrders")]
        public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] CreatePurchaseOrderDto dto)
        {
            var result = await sender.Send(new CreatePurchaseOrderCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        [RedisCache(300)]
        public async Task<ActionResult<PaginatedResult<PurchaseOrderDto>>> GetAll([FromQuery] PurchaseOrdersQueryParameters queryParameters)
        {
            var result = await sender.Send(new GetAllPurchaseOrdersQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<ActionResult<PurchaseOrderDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetPurchaseOrderByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/PurchaseOrders")]
        public async Task<ActionResult<PurchaseOrderDto>> Update(Guid id, [FromBody] UpdatePurchaseOrderDto dto)
        {
            var result = await sender.Send(new UpdatePurchaseOrderCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [CacheInvalidate("/api/PurchaseOrders")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader(Name = "If-Match")] string rowVersionStr)
        {
            if (string.IsNullOrEmpty(rowVersionStr))
                return BadRequest("Row version (If-Match header) is required.");

            byte[] rowVersion = Convert.FromBase64String(rowVersionStr);

            await sender.Send(new TogglePurchaseOrderCommand(id, CurrentUserId, rowVersion));
            return NoContent();
        }
    }
}