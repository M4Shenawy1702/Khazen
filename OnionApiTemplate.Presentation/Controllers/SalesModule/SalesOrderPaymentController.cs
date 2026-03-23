using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete;
using Khazen.Application.UseCases.SalesModule.SalesOrderPaymentUseCases.Queries.GetAll;
using Khazen.Application.UseCases.SalesModule.SalesOrderPaymentUseCases.Queries.GetById;
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
    public class SalesOrderPaymentController(ISender mediator) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPost]
        [CacheInvalidate("/api/SalesOrderPayment")]
        [CacheInvalidate("/api/SalesOrders")]
        [CacheInvalidate("/api/SalesInvoices")]
        [CacheInvalidate("/api/Customer")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<IActionResult> Create([FromBody] CreateSalesInvoicePaymentDto dto)
        {
            var result = await mediator.Send(new CreateSalesInvoicePaymentCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPatch("reverse/{id:guid}")]
        [CacheInvalidate("/api/SalesOrderPayment")]
        [CacheInvalidate("/api/SalesOrders")]
        [CacheInvalidate("/api/SalesInvoices")]
        [CacheInvalidate("/api/Customer")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<IActionResult> Reverse(Guid id, [FromHeader(Name = "If-Match")] string rowVersionStr)
        {
            if (string.IsNullOrEmpty(rowVersionStr))
                return BadRequest("If-Match header with row version is required for reversal.");

            var rowVersion = Convert.FromBase64String(rowVersionStr);
            var result = await mediator.Send(new ReverseSalesInvoicePaymentCommand(id, rowVersion, CurrentUserId));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await mediator.Send(new GetSalesInvoicePaymentByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet]
        [RedisCache(60)]
        public async Task<IActionResult> GetAll([FromQuery] SalesOrderPaymentQueryParameters queryParameters)
        {
            var result = await mediator.Send(new GetAllSalesInvoicePaymentsQuery(queryParameters));
            return Ok(result);
        }
    }
}