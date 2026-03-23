using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetById;
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
    public class CategoryController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(600)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var result = await sender.Send(new GetAllCategoriesQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(600)]
        public async Task<ActionResult<CategoryDetailsDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetCategoryByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Category")]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await sender.Send(new CreateCategoryCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Category")]
        [CacheInvalidate("/api/Items")]
        public async Task<ActionResult<CategoryDetailsDto>> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await sender.Send(new UpdateCategoryCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("toggle/{id:guid}")]
        [CacheInvalidate("/api/Category")]
        public async Task<ActionResult> Toggle(Guid id)
        {
            await sender.Send(new ToggleCategoryCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}