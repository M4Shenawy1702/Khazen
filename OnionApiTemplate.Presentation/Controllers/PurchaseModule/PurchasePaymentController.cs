using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Api.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Finance,Admin")]
    public class PurchasePaymentController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                                ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(120)]
        public async Task<ActionResult<PaginatedResult<PurchasePaymentDto>>> GetAll([FromQuery] PurchasePaymentQueryParameters queryParameters)
        {
            var result = await sender.Send(new GetAllPurchasePaymentsQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<ActionResult<PurchasePaymentDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetPurchasePaymentByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/PurchasePayment")]
        [CacheInvalidate("/api/Accounts")]
        [CacheInvalidate("/api/PurchaseInvoice")]
        public async Task<ActionResult<PurchasePaymentDto>> Create([FromBody] CreatePurchasePaymentDto dto)
        {
            var result = await sender.Send(new CreatePurchasePaymentCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        [CacheInvalidate("/api/PurchasePayment")]
        [CacheInvalidate("/api/Accounts")]
        [CacheInvalidate("/api/PurchaseInvoice")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            if (string.IsNullOrEmpty(rowVersion))
                return BadRequest("Row version (If-Match) is required for payment reversal.");

            var versionBytes = Convert.FromBase64String(rowVersion);

            await sender.Send(new ReversePurchasePaymentCommand(id, versionBytes, CurrentUserId));

            return NoContent();
        }
    }
}