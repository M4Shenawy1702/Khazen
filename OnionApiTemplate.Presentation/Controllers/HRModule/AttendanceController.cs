using Khazen.Application.Common.QueryParameters;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Absent;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckIn;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckOut;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Update;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AttendanceQueryParameters queryParameters)
        {
            var result = await _sender.Send(new GetAllAttendanceQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new GetAttendanceByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAttendanceCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }


        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInAttendanceCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetAll), null, result);
        }
        [HttpPost("absent")]
        public async Task<IActionResult> Absent(AbsentCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetAll), null, result);
        }
        [HttpPut("check-out/{id}")]
        public async Task<IActionResult> CheckOut(Guid id)
        {
            var result = await _sender.Send(new CheckOutCommand(id));
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sender.Send(new ToggleAttendanceCommand(id));
            return NoContent();
        }

    }
}
