using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseReceiptsController(IMediator mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                       ?? throw new UnauthorizedAccessException("User identity not available.");
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PurchaseReceiptDto>>> GetAll([FromQuery] PurchaseReceiptsQueryParameters queryParameters)
            => Ok(await _mediator.Send(new GetAllPurchaseReceiptsQuery(queryParameters)));


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseReceiptDto>> GetById(Guid id)
            => Ok(await _mediator.Send(new GetPurchaseReceiptByIdQuery(id)));

        [HttpPost]
        public async Task<ActionResult<PurchaseReceiptDto>> Create([FromBody] CreatePurchaseReceiptDto dto)
        {
            var command = new CreatePurchaseReceiptCommand(dto, CurrentUserId);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PurchaseReceiptDto>> Update(Guid id, [FromBody] UpdatePurchaseReceiptDto dto)
        {
            return Ok(await _mediator.Send(new UpdatePurchaseReceiptCommand(id, dto, CurrentUserId)));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader(Name = "RowVersion")] string rowVersionStr)
        {
            var rowVersion = Convert.FromBase64String(rowVersionStr);
            return Ok(await _mediator.Send(new DeletePurchaseReceiptCommand(id, CurrentUserId, rowVersion)));
        }
    }
}
