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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseOrdersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                              ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] CreatePurchaseOrderDto dto)
        {
            var result = await _mediator.Send(new CreatePurchaseOrderCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PurchaseOrderDto>>> GetAll([FromQuery] PurchaseOrdersQueryParameters queryParameters)
        {
            return Ok(await _mediator.Send(new GetAllPurchaseOrdersQuery(queryParameters)));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseOrderDto>> GetById(Guid id)
        {
            return Ok(await _mediator.Send(new GetPurchaseOrderByIdQuery(id)));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PurchaseOrderDto>> Update(Guid id, [FromBody] UpdatePurchaseOrderDto dto)
        {
            var result = await _mediator.Send(new UpdatePurchaseOrderCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader(Name = "If-Match")] string rowVersionStr)
        {
            if (CurrentUserId is null) return Unauthorized();

            byte[] rowVersion = Convert.FromBase64String(rowVersionStr);

            await _mediator.Send(new TogglePurchaseOrderCommand(id, CurrentUserId, rowVersion));
            return NoContent();
        }
    }
}