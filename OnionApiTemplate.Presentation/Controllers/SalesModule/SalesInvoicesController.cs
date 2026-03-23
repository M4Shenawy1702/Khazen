using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Update;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Void;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetAll;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.SalesModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalesInvoicesController(ISender mediator) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.Identity?.Name
            ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(300)]
        public async Task<IActionResult> GetAllInvoices([FromQuery] SalesInvoicesQueryParameters queryParameters)
        {
            var result = await mediator.Send(new GetAllSalesInvoicesQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(600)]
        public async Task<IActionResult> GetInvoiceById(Guid id)
        {
            var result = await mediator.Send(new GetSalesInvoiceByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/SalesInvoices")]
        [CacheInvalidate("/api/SalesOrders")]
        [CacheInvalidate("/api/Customer")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateSalesInvoiceDto dto)
        {
            var result = await mediator.Send(new CreateSalesInvoiceCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetInvoiceById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/SalesInvoices")]
        public async Task<IActionResult> UpdateInvoice(Guid id, [FromBody] UpdateSalesInvoiceDto dto, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            if (string.IsNullOrEmpty(rowVersion)) return BadRequest("If-Match header is required.");

            var rowVersionBytes = Convert.FromBase64String(rowVersion);
            var result = await mediator.Send(new UpdateSalesInvoiceCommand(id, dto, rowVersionBytes, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("{id:guid}/void")]
        [CacheInvalidate("/api/SalesInvoices")]
        [CacheInvalidate("/api/Customer")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<IActionResult> VoidInvoice(Guid id, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            if (string.IsNullOrEmpty(rowVersion)) return BadRequest("If-Match header is required.");

            var rowVersionBytes = Convert.FromBase64String(rowVersion);
            var result = await mediator.Send(new VoidSalesInvoiceCommand(id, rowVersionBytes, CurrentUserId));
            return Ok(result);
        }
    }
}