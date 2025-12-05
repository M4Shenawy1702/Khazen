using Khazen.Application.Common.QueryParameters;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Add;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.InventoryModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductsQueryParameters queryParameters)
        {
            var query = new GetAllProductsQuery(queryParameters);
            var result = await _sender.Send(query);
            return Ok(result);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetProductByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddProductCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _sender.Send(new DeleteProductCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
