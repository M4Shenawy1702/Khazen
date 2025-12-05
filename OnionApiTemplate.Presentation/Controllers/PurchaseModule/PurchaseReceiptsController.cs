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

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseReceiptsController : ControllerBase
    {
        private readonly ISender _mediator;

        public PurchaseReceiptsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PurchaseReceiptDto>>> GetAll([FromQuery] PurchaseReceiptsQueryParameters queryParameters)
            => Ok(await _mediator.Send(new GetAllPurchaseReceiptsQuery(queryParameters)));


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseReceiptDto>> GetById(Guid id)
            => Ok(await _mediator.Send(new GetPurchaseReceiptByIdQuery(id)));

        [HttpPost]
        public async Task<ActionResult<PurchaseReceiptDto>> Create([FromBody] CreatePurchaseReceiptDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var command = new CreatePurchaseReceiptCommand(dto, user);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PurchaseReceiptDto>> Update(Guid id, [FromBody] UpdatePurchaseReceiptDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            return Ok(await _mediator.Send(new UpdatePurchaseReceiptCommand(id, dto, user)));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader] byte[] RowVersion)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            return Ok(await _mediator.Send(new DeletePurchaseReceiptCommand(id, user, RowVersion)));
        }
    }
}
