using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.InventoryModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [RedisCache(600)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var result = await _sender.Send(new GetAllCategoriesQuery());
            return Ok(result);
        }

        [RedisCache(600)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoryDetailsDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetCategoryByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Category")]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await _sender.Send(new CreateCategoryCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Category")]
        public async Task<ActionResult<CategoryDetailsDto>> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _sender.Send(new UpdateCategoryCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("Toggle/{id:guid}")]
        [CacheInvalidate("/api/Category")]
        public async Task<ActionResult> Toggle(Guid id)
        {
            await _sender.Send(new ToggleCategoryCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}
