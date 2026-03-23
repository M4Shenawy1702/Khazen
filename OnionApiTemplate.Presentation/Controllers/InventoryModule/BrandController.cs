using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetById;
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
    public class BrandController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(600)]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
        {
            var result = await sender.Send(new GetAllBrandsQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(600)]
        public async Task<ActionResult<BrandDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetBrandByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Brand")]
        public async Task<ActionResult<BrandDetailsDto>> Create([FromBody] CreateBrandDto dto)
        {
            var result = await sender.Send(new CreateBrandCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Brand")]
        [CacheInvalidate("/api/Items")]
        public async Task<ActionResult<BrandDetailsDto>> Update(Guid id, [FromBody] UpdateBrandDto dto)
        {
            var result = await sender.Send(new UpdateBrandCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("toggle/{id:guid}")]
        [CacheInvalidate("/api/Brand")]
        public async Task<ActionResult> Toggle(Guid id)
        {
            await sender.Send(new ToggleBrandCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}