using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Delete;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Update;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetAll;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.PurchaseModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SupplierController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(600)]
        public async Task<ActionResult<List<SupplierDto>>> GetAll()
        {
            var result = await sender.Send(new GetAllSuppliersQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(600)]
        public async Task<ActionResult<SupplierDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetSupplierByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Supplier")]
        public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierDto dto)
        {
            var result = await sender.Send(new CreateSupplierCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Supplier")]
        [CacheInvalidate("/api/PurchaseOrders")]
        [CacheInvalidate("/api/PurchaseInvoice")]
        public async Task<ActionResult<SupplierDto>> Update(Guid id, [FromBody] UpdateSupplierDto dto)
        {
            var result = await sender.Send(new UpdateSupplierCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [CacheInvalidate("/api/Supplier")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await sender.Send(new ToggleSupplierCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}