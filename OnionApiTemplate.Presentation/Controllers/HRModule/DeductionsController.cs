using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.DeductionDtos;
using Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Add;
using Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeductionsController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                         ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DeductionQueryParameters queryParameters)
        {
            var result = await _sender.Send(new GetAllDeductionsQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetDeductionByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddDeductionDto dto)
        {
            var result = await _sender.Send(new AddDeductionCommand(dto, CurrentUserId));

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            await _sender.Send(new ToggleDeductionCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}