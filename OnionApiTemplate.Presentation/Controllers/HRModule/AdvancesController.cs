using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.AdvanceDtos;
using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Toggle;
using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Queries.GetAll;
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
    public class AdvancesController(ISender mediator) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPost]
        [CacheInvalidate("/api/Advances")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<ActionResult<AdvanceDto>> AddAdvance([FromBody] AddAdvanceDto dto)
        {
            var result = await mediator.Send(new AddAdvanceCommand(dto, CurrentUserId));

            return Ok(result);
        }

        [HttpGet]
        [RedisCache(120)]
        public async Task<ActionResult<List<AdvanceDto>>> GetAllAdvances([FromQuery] AdvanceQueryParameters queryParameters)
        {
            var result = await mediator.Send(new GetAllAdvanceQuery(queryParameters));
            return Ok(result);
        }

        [HttpPatch("toggle/{id:int}")]
        [CacheInvalidate("/api/Advances")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<IActionResult> ToggleAdvance(int id, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            var rowVersionBytes = Convert.FromBase64String(rowVersion);

            var result = await mediator.Send(new ToggleAdvanceCommand(id, rowVersionBytes, CurrentUserId));

            return Ok(result);
        }
    }
}