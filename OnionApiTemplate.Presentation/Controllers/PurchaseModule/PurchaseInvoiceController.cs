using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseInvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchaseInvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PurchaseInvoiceDto>>> GetAll([FromQuery] PurchaseInvoiceQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllPurchaseInvoicesQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseInvoiceDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPurchaseInvoiceByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseInvoiceDto>> Create([FromBody] CreatePurchaseInvoiceDto dto)
        {
            var createdBy = User.Identity!.Name;
            if (createdBy is null) return BadRequest("User not found");
            var result = await _mediator.Send(new CreateInvoiceForReceiptCommand(dto, createdBy));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PurchaseInvoiceDto>> Update(Guid id, [FromBody] UpdatePurchaseInvoiceDto dto)
        {
            var modifiedBy = User.Identity!.Name;
            if (modifiedBy is null) return BadRequest("User not found");
            var result = await _mediator.Send(new UpdatePurchaseInvoiceCommand(id, dto, modifiedBy));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] byte[] RowVersion)
        {
            var modifiedBy = User.Identity!.Name;
            if (modifiedBy is null) return BadRequest("User not found");
            await _mediator.Send(new DeletePurchaseInvoiceCommand(id, modifiedBy, RowVersion));
            return NoContent();
        }
    }
}
