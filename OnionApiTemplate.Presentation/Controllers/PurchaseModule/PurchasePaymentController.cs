using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Api.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasePaymentController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                              ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PurchasePaymentDto>>> GetAll([FromQuery] PurchasePaymentQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllPurchasePaymentsQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchasePaymentDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPurchasePaymentByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PurchasePaymentDto>> Create([FromBody] CreatePurchasePaymentDto dto)
        {
            var result = await _mediator.Send(new CreatePurchasePaymentCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader(Name = "RowVersion")] string rowVersion)
        {
            var versionBytes = Convert.FromBase64String(rowVersion);
            await _mediator.Send(new ReversePurchasePaymentCommand(id, versionBytes, CurrentUserId));
            return NoContent();
        }
    }
}
