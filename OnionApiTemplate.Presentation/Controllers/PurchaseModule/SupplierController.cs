using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

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
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new CreateSupplierCommand(dto, user));
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SupplierDto>> Update([FromBody] UpdateSupplierDto dto, Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new UpdateSupplierCommand(id, dto, user));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            await _mediator.Send(new ToggleSupplierCommand(id, user));
            return NoContent();
        }
    }
}
