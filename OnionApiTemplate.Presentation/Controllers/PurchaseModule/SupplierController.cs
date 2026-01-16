using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                       ?? throw new UnauthorizedAccessException("User identity not available.");
        [HttpGet]
        public async Task<ActionResult<List<SupplierDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllSuppliersQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SupplierDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSupplierByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierDto dto)
        {
            var result = await _mediator.Send(new CreateSupplierCommand(dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SupplierDto>> Update([FromBody] UpdateSupplierDto dto, Guid id)
        {
            var result = await _mediator.Send(new UpdateSupplierCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new ToggleSupplierCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}
