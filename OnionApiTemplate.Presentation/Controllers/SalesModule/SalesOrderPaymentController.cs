using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete;
using Khazen.Application.UseCases.SalesModule.SalesOrderPaymentUseCases.Queries.GetAll;
using Khazen.Application.UseCases.SalesModule.SalesOrderPaymentUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.SalesModule
{

    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrderPaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesOrderPaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSalesInvoicePaymentDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new CreateSalesInvoicePaymentCommand(dto, user));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, byte[] rowVersion)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new DeleteSalesInvoicePaymentCommand(id, rowVersion, user));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSalesInvoicePaymentByIdQuery(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SalesOrderPaymentQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllSalesInvoicePaymentsQuery(queryParameters));
            return Ok(result);
        }
    }
}
