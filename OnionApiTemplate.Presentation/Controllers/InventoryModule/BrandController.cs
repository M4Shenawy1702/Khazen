using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.InventoryModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly ISender _sender;

        public BrandController(ISender sender)
        {
            _sender = sender;
        }
        [RedisCache]
        [HttpGet]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
        {
            var result = await _sender.Send(new GetAllBrandsQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BrandDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetBrandByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<BrandDetailsDto>> Create([FromBody] CreateBrandCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut]
        public async Task<ActionResult<BrandDetailsDto>> Update([FromBody] UpdateBrandCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var createdBy = User?.Identity?.Name;
            if (createdBy == null) return BadRequest("User not found");
            var result = await _sender.Send(new ToggleBrandCommand(id, createdBy));
            return result ? NoContent() : NotFound();
        }
    }
}
