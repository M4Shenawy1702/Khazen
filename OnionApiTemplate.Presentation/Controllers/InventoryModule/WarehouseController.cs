using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.InventoryModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WarehouseController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPost]
        [CacheInvalidate("/api/Warehouse")]
        public async Task<ActionResult<WarehouseDto>> Create([FromBody] CreateWarehouseDto dto)
        {
            var result = await sender.Send(new CreateWarehouseCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        [RedisCache(600)]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAll()
        {
            var result = await sender.Send(new GetAllWarehousesQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(600)]
        public async Task<ActionResult<WarehouseDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetWarehouseByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Warehouse")]
        [CacheInvalidate("/api/Product")]
        public async Task<ActionResult<WarehouseDto>> Update(Guid id, [FromBody] UpdateWarehouseDto dto)
        {
            var result = await sender.Send(new UpdateWarehouseCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("toggle/{id:guid}")]
        [CacheInvalidate("/api/Warehouse")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            await sender.Send(new ToggleWarehouseCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}