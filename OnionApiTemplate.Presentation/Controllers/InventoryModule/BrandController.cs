using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.InventoryModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [RedisCache(600)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
        {
            var result = await _sender.Send(new GetAllBrandsQuery());
            return Ok(result);
        }

        [RedisCache(600)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BrandDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetBrandByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Brand")]
        public async Task<ActionResult<BrandDetailsDto>> Create([FromBody] CreateBrandDto dto)
        {
            var result = await _sender.Send(new CreateBrandCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Brand")]
        public async Task<ActionResult<BrandDetailsDto>> Update(Guid id, [FromBody] UpdateBrandDto dto)
        {
            var result = await _sender.Send(new UpdateBrandCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("Toggle/{id:guid}")]
        [CacheInvalidate("/api/Brand")]
        public async Task<ActionResult> Toggle(Guid id)
        {
            await _sender.Send(new ToggleBrandCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}
