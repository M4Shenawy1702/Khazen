using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.Employee;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Commands.Update;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetById;
using Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.CreateEmployee;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(ISender mediator)
                : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] EmployeeQueryParameters queryParameters)
        {
            var query = new GetAllEmployeesQuery(queryParameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            var query = new GetEmployeeByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto Dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new CreateEmployeeCommand(Dto, user));
            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Id }, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto Dto, Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new UpdateEmployeeCommand(id, Dto, user));
            return Ok(result);
        }
    }
}
