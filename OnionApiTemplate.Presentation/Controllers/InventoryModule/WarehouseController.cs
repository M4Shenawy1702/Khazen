using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.InventoryModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<WarehouseDto>> Create([FromBody] CreateWarehouseCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllWarehousesQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WarehouseDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetWarehouseByIdQuery(id));
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WarehouseDto>> Update(Guid id, [FromBody] UpdateWarehouseCommand command)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            await _mediator.Send(new ToggleWarehouseCommand(id, user));
            return NoContent();
        }
    }
}
