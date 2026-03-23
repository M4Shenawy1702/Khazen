using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Reverse;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetById;
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
    public class PurchaseInvoiceController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(300)]
        public async Task<ActionResult<PaginatedResult<PurchaseInvoiceDto>>> GetAll([FromQuery] PurchaseInvoiceQueryParameters queryParameters)
        {
            var result = await sender.Send(new GetAllPurchaseInvoicesQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<ActionResult<PurchaseInvoiceDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetPurchaseInvoiceByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/PurchaseInvoice")]
        [CacheInvalidate("/api/Accounts")]
        [CacheInvalidate("/api/Product")]
        public async Task<ActionResult<PurchaseInvoiceDto>> Create([FromBody] CreatePurchaseInvoiceDto dto)
        {
            var result = await sender.Send(new CreateInvoiceForReceiptCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/PurchaseInvoice")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<ActionResult<PurchaseInvoiceDto>> Update(Guid id, [FromBody] UpdatePurchaseInvoiceDto dto)
        {
            var result = await sender.Send(new UpdatePurchaseInvoiceCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("reverse/{id:guid}")]
        [CacheInvalidate("/api/PurchaseInvoice")]
        [CacheInvalidate("/api/Accounts")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> Reverse(
                    Guid id,
                    [FromHeader(Name = "If-Match")] string rowVersion)
        {
            if (string.IsNullOrEmpty(rowVersion))
            {
                return BadRequest("Row version (If-Match header) is required for reversal.");
            }

            var rowVersionBytes = Convert.FromBase64String(rowVersion);

            var command = new ReversePurchaseInvoiceCommand(id, CurrentUserId, rowVersionBytes);
            await sender.Send(command);

            return NoContent();
        }
    }
}