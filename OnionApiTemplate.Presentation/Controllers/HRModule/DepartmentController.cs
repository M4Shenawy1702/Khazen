using Khazen.Application.Common.QueryParameters;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Update;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController(ISender sender)
                : ControllerBase
    {
        private readonly ISender _Sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DepartmentQueryParameters queryParameters)
        {
            var query = new GetAllDepartmentQuery(queryParameters);
            var departments = await _Sender.Send(query);
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetDepartmentByIdQuery(id);
            var department = await _Sender.Send(query);
            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand createCommand)
        {
            var createdDepartment = await _Sender.Send(createCommand);
            return CreatedAtAction(nameof(GetById), new { id = createdDepartment.Id }, createdDepartment);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDepartmentCommand updateCommand)
        {
            var updatedResult = await _Sender.Send(updateCommand);
            return Ok(updatedResult);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var command = new ToggleDepartmentCommand(id, user);
            var deleted = await _Sender.Send(command);
            return Ok(deleted);
        }
    }
}
