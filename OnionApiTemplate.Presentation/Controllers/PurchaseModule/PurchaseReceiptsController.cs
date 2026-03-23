using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetById;
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
    public class PurchaseReceiptsController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(120)]
        public async Task<ActionResult<PaginatedResult<PurchaseReceiptDto>>> GetAll([FromQuery] PurchaseReceiptsQueryParameters queryParameters)
        {
            var result = await sender.Send(new GetAllPurchaseReceiptsQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<ActionResult<PurchaseReceiptDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetPurchaseReceiptByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/PurchaseReceipts")]
        [CacheInvalidate("/api/Product")]
        [CacheInvalidate("/api/Warehouse")]
        [CacheInvalidate("/api/PurchaseOrders")]
        public async Task<ActionResult<PurchaseReceiptDto>> Create([FromBody] CreatePurchaseReceiptDto dto)
        {
            var result = await sender.Send(new CreatePurchaseReceiptCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/PurchaseReceipts")]
        [CacheInvalidate("/api/Product")]
        public async Task<ActionResult<PurchaseReceiptDto>> Update(Guid id, [FromBody] UpdatePurchaseReceiptDto dto)
        {
            var result = await sender.Send(new UpdatePurchaseReceiptCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [CacheInvalidate("/api/PurchaseReceipts")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader(Name = "If-Match")] string rowVersionStr)
        {
            if (string.IsNullOrEmpty(rowVersionStr))
                return BadRequest("Row version (If-Match header) is required.");

            var rowVersion = Convert.FromBase64String(rowVersionStr);
            await sender.Send(new DeletePurchaseReceiptCommand(id, CurrentUserId, rowVersion));

            return NoContent();
        }
    }
}