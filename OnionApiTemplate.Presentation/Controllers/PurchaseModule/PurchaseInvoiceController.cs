using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Reverse;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseInvoiceController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PurchaseInvoiceDto>>> GetAll([FromQuery] PurchaseInvoiceQueryParameters queryParameters)
        {
            var result = await _sender.Send(new GetAllPurchaseInvoicesQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseInvoiceDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetPurchaseInvoiceByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseInvoiceDto>> Create([FromBody] CreatePurchaseInvoiceDto dto)
        {
            var result = await _sender.Send(new CreateInvoiceForReceiptCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PurchaseInvoiceDto>> Update(Guid id, [FromBody] UpdatePurchaseInvoiceDto dto)
        {
            var result = await _sender.Send(new UpdatePurchaseInvoiceCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPut("reverse/{id:guid}")]
        public async Task<IActionResult> Reverse(Guid id, [FromQuery] byte[] RowVersion)
        {
            await _sender.Send(new ReversePurchaseInvoiceCommand(id, CurrentUserId, RowVersion));
            return NoContent();
        }
    }
}
