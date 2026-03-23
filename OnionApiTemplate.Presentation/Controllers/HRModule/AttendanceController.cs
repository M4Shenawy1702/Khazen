using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Absent;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckIn;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckOut;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Leave;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Update;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User identity not found.");

        [HttpGet]
        [RedisCache(120)]
        public async Task<IActionResult> GetAll([FromQuery] AttendanceQueryParameters queryParameters)
        {
            var result = await sender.Send(new GetAllAttendanceQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await sender.Send(new GetAttendanceByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("check-in")]
        [CacheInvalidate("/api/Attendance")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            var result = await sender.Send(new CheckInAttendanceCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("check-out")]
        [CacheInvalidate("/api/Attendance")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
        {
            var result = await sender.Send(new CheckOutCommand(dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Attendance")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceDto dto)
        {
            var result = await sender.Send(new UpdateAttendanceCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPost("absent")]
        [CacheInvalidate("/api/Attendance")]
        public async Task<IActionResult> Absent([FromBody] AbsentDto dto)
        {
            var result = await sender.Send(new AbsentCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("leave")]
        [CacheInvalidate("/api/Attendance")]
        public async Task<IActionResult> MarkAsLeave([FromBody] MarkAsLeaveDto dto)
        {
            var result = await sender.Send(new MarkAsLeaveCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        [CacheInvalidate("/api/Attendance")]
        public async Task<IActionResult> ToggleDelete(Guid id)
        {
            await sender.Send(new ToggleAttendanceCommand(id, CurrentUserId));
            return NoContent();
        }
    }
}