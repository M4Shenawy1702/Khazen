using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.Employee;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Commands.Update;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetById;
using Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<EmployeeDto>>> GetEmployees([FromQuery] EmployeeQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllEmployeesQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeDetailsDto>> GetEmployeeById(Guid id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            var result = await _mediator.Send(new CreateEmployeeCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmployeeDetailsDto>> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            var result = await _mediator.Send(new UpdateEmployeeCommand(id, dto, CurrentUserId));
            return Ok(result);
        }
    }
}