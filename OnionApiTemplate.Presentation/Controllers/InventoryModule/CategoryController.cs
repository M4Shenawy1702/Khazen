using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.InventoryModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ISender _sender;

        public CategoryController(ISender sender)
        {
            _sender = sender;
        }
        [RedisCache]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var result = await _sender.Send(new GetAllCategoriesQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoryDetailsDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetCategoryByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CategoryDetailsDto>> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _sender.Send(new UpdateCategoryCommand(id, dto));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _sender.Send(new DeleteCategoryCommand(id));
            return result;
        }
    }
}
