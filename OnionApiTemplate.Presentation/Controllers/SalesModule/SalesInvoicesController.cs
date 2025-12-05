using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Update;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Void;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetAll;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.SalesModule
{
    [ApiController]
    [Route("api/salesinvoices")]
    public class SalesInvoicesController : ControllerBase
    {
        private readonly ISender _mediator;

        public SalesInvoicesController(ISender mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices([FromQuery] SalesInvoicesQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllSalesInvoicesQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(Guid id)
        {
            var result = await _mediator.Send(new GetSalesInvoiceByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateSalesInvoiceDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new CreateSalesInvoiceCommand(dto, user));
            return CreatedAtAction(nameof(GetInvoiceById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(Guid id, [FromBody] UpdateSalesInvoiceDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new UpdateSalesInvoiceCommand(id, dto, user));
            return Ok(result);
        }

        [HttpPatch("Void/{id}")]
        public async Task<IActionResult> VoidInvoice(Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new VoidSalesInvoiceCommand(id, user));
            return Ok(result);
        }

    }
}
