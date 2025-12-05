using Khazen.Application.Common.QueryParameters;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Update;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR")]
    public class PerformanceReviewsController : ControllerBase
    {
        private readonly ISender _sender;

        public PerformanceReviewsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePerformanceReviewCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PerformanceReviewsQueryParameters parameters)
        {
            var result = await _sender.Send(new GetAllPerformanceReviewsQuery(parameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetPerformanceReviewByIdQuery(id));
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePerformanceReviewCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            await _sender.Send(new TogglePerformanceReviewCommand(id, user));
            return NoContent();
        }
    }
}
