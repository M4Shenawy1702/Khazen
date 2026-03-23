using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.BonusDtos;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Add;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BonusesController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPost]
        [CacheInvalidate("/api/Bonuses")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<ActionResult<BonusDto>> GrantBonus([FromBody] AddBonusDto dto)
        {
            var command = new AddBonusCommand(dto, CurrentUserId);
            var result = await sender.Send(command);
            return CreatedAtAction(nameof(GetBonusById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}", Name = nameof(GetBonusById))]
        [RedisCache(300)]
        public async Task<ActionResult<BonusDetailsDto>> GetBonusById(Guid id)
        {
            var result = await sender.Send(new GetBonusByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet]
        [RedisCache(120)]
        public async Task<ActionResult<PaginatedResult<BonusDto>>> GetAllBonuses([FromQuery] BonusQueryParameters queryParameters)
        {
            var result = await sender.Send(new GetAllBounsQuery(queryParameters));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [CacheInvalidate("/api/Bonuses")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<IActionResult> ToggleDeleteBonus(Guid id)
        {
            var command = new ToggleBonusCommand(id, CurrentUserId);
            await sender.Send(command);
            return NoContent();
        }
    }
}