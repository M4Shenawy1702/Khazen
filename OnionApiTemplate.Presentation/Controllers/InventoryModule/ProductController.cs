using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Add;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Delete;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Update;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetAll;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetById;
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
    public class ProductController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(300)]
        public async Task<IActionResult> GetAll([FromQuery] ProductsQueryParameters queryParameters)
        {
            var result = await sender.Send(new GetAllProductsQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await sender.Send(new GetProductByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Product")]
        [CacheInvalidate("/api/Category")]
        public async Task<IActionResult> Create([FromForm] AddProductDto dto)
        {
            var result = await sender.Send(new AddProductCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductDto dto)
        {
            var result = await sender.Send(new UpdateProductCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("toggle/{id:guid}")]
        [CacheInvalidate("/api/Product")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            await sender.Send(new ToggleProductCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}