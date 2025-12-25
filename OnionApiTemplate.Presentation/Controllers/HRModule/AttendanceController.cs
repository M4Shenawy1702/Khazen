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
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not found.");
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AttendanceQueryParameters queryParameters)
        {
            var result = await _sender.Send(new GetAllAttendanceQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetAttendanceByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            var command = new CheckInAttendanceCommand(dto, CurrentUserId);
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("check-out")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
        {
            var command = new CheckOutCommand(dto, CurrentUserId);

            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceDto dto)
        {
            var command = new UpdateAttendanceCommand(id, dto, CurrentUserId);
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPost("absent")]
        public async Task<IActionResult> Absent([FromBody] AbsentDto dto)
        {
            var command = new AbsentCommand(dto, CurrentUserId);
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("leave")]
        public async Task<IActionResult> MarkAsLeave([FromBody] MarkAsLeaveDto dto)
        {
            var command = new MarkAsLeaveCommand(dto, CurrentUserId);
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> ToggleDelete(Guid id)
        {
            var command = new ToggleAttendanceCommand(id, CurrentUserId);
            await _sender.Send(command);
            return NoContent();
        }
    }
}