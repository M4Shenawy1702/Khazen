using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Application.DOTs.HRModule.DepartmentDtos;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Update;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<DepartmentDto>>> GetAll([FromQuery] DepartmentQueryParameters queryParameters)
        {
            var result = await _sender.Send(new GetAllDepartmentQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DepartmentDetailsDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetDepartmentByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDetailsDto>> Create([FromBody] CreateDepartmentDto dto)
        {
            var result = await _sender.Send(new CreateDepartmentCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<DepartmentDetailsDto>> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
        {
            var result = await _sender.Send(new UpdateDepartmentCommand(id, dto, CurrentUserId));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> Toggle(Guid id)
        {
            var result = await _sender.Send(new ToggleDepartmentCommand(id, CurrentUserId));
            return Ok(result);
        }
    }
}