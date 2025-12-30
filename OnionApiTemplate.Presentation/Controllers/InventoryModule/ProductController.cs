using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Add;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.InventoryModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [RedisCache(600)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductsQueryParameters queryParameters)
        {
            var result = await _sender.Send(new GetAllProductsQuery(queryParameters));
            return Ok(result);
        }

        [RedisCache(600)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetProductByIdQuery(id));

            return Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> Create([FromForm] AddProductDto dto)
        {
            var result = await _sender.Send(new AddProductCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductDto dto)
        {
            var result = await _sender.Send(new UpdateProductCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("Toggle/{id:guid}")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            await _sender.Send(new ToggleProductCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}
