using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchaseOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] CreatePurchaseOrderDto dto)
        {
            var createdBy = User.Identity!.Name;
            if (createdBy is null) return BadRequest("User not found");
            var result = await _mediator.Send(new CreatePurchaseOrderCommand(dto, createdBy));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PurchaseOrderDto>>> GetAll([FromQuery] PurchaseOrdersQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllPurchaseOrdersQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseOrderDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PurchaseOrderDto>> Update(Guid id, [FromBody] UpdatePurchaseOrderDto dto)
        {
            var createdBy = User.Identity!.Name;
            if (createdBy is null) return BadRequest("User not found");
            var result = await _mediator.Send(new UpdatePurchaseOrderCommand(id, dto, createdBy));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader] byte[] RowVersion)
        {
            var createdBy = User.Identity!.Name;
            if (createdBy is null) return BadRequest("User not found");
            await _mediator.Send(new TogglePurchaseOrderCommand(id, createdBy, RowVersion));
            return NoContent();
        }
    }
}
