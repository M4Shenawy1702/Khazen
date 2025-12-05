using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Api.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasePaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchasePaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

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
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new CreatePurchasePaymentCommand(dto, user));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            await _mediator.Send(new DeletePurchasePaymentCommand(id, user));
            return NoContent();
        }
    }
}
