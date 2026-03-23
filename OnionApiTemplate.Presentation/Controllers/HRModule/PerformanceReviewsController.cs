using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.PerformanceReviewDtos;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Update;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR,Manager")]
    public class PerformanceReviewsController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPost]
        [CacheInvalidate("/api/PerformanceReviews")]
        [CacheInvalidate("/api/Employee")]
        public async Task<ActionResult<PerformanceReviewDto>> Create([FromBody] CreatePerformanceReviewDto dto)
        {
            var result = await sender.Send(new CreatePerformanceReviewCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        [RedisCache(120)]
        public async Task<ActionResult<PaginatedResult<PerformanceReviewDto>>> GetAll([FromQuery] PerformanceReviewsQueryParameters parameters)
        {
            var result = await sender.Send(new GetAllPerformanceReviewsQuery(parameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<ActionResult<PerformanceReviewDto>> GetById(Guid id)
        {
            var result = await sender.Send(new GetPerformanceReviewByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/PerformanceReviews")]
        public async Task<ActionResult<PerformanceReviewDto>> Update(Guid id, [FromBody] UpdatePerformanceReviewDto dto)
        {
            var result = await sender.Send(new UpdatePerformanceReviewCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [CacheInvalidate("/api/PerformanceReviews")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await sender.Send(new TogglePerformanceReviewCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}