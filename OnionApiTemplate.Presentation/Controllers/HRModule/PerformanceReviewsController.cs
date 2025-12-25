using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.PerformanceReviewDtos;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Update;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "HR")]
public class PerformanceReviewsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                    ?? throw new UnauthorizedAccessException("User identity not available.");

    [HttpPost]
    public async Task<ActionResult<PerformanceReviewDto>> Create([FromBody] CreatePerformanceReviewDto dto)
    {
        var result = await _sender.Send(new CreatePerformanceReviewCommand(dto, CurrentUserId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PerformanceReviewDto>>> GetAll([FromQuery] PerformanceReviewsQueryParameters parameters)
    {
        var result = await _sender.Send(new GetAllPerformanceReviewsQuery(parameters));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PerformanceReviewDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetPerformanceReviewByIdQuery(id));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PerformanceReviewDto>> Update(Guid id, [FromBody] UpdatePerformanceReviewDto dto)
    {
        var result = await _sender.Send(new UpdatePerformanceReviewCommand(id, dto, CurrentUserId));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new TogglePerformanceReviewCommand(id, CurrentUserId));
        return NoContent();
    }
}