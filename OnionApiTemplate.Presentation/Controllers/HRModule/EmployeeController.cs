using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.Employee;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Commands.Update;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetById;
using Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.Create;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController(ISender mediator) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        [RedisCache(300)]
        public async Task<ActionResult<PaginatedResult<EmployeeDto>>> GetEmployees([FromQuery] EmployeeQueryParameters queryParameters)
        {
            var result = await mediator.Send(new GetAllEmployeesQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(600)]
        public async Task<ActionResult<EmployeeDetailsDto>> GetEmployeeById(Guid id)
        {
            var result = await mediator.Send(new GetEmployeeByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [CacheInvalidate("/api/Employee")]
        [CacheInvalidate("/api/Department")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            var result = await mediator.Send(new CreateEmployeeCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [CacheInvalidate("/api/Employee")]
        public async Task<ActionResult<EmployeeDetailsDto>> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            var result = await mediator.Send(new UpdateEmployeeCommand(id, dto, CurrentUserId));
            return Ok(result);
        }
    }
}