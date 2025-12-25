using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.SalaryDots;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetById;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetPayslip;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;


        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                   ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet("{salaryId:guid}")]
        public async Task<IActionResult> GetSalaryById(Guid salaryId)
        {
            var dto = await _sender.Send(new GetSalaryByIdQuery(salaryId));
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalaries([FromQuery] SalariesQueryParameters queryParameters)
        {
            var query = new GetAllSalariesQuery(queryParameters);
            var paged = await _sender.Send(query);
            return Ok(paged);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalary([FromBody] CreateSalaryDto Dto)
        {
            var created = await _sender.Send(new CreateSalaryCommand(Dto, CurrentUserId));
            return CreatedAtAction(nameof(GetSalaryById), new { salaryId = created.Id }, created);
        }

        [HttpDelete("{salaryId:guid}")]
        public async Task<IActionResult> DeleteSalary(Guid salaryId)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _sender.Send(new DeleteSalaryCommand(salaryId, user));
            return NoContent();
        }

        [HttpGet("{salaryId:guid}/payslip")]
        public async Task<IActionResult> GetPayslip(Guid salaryId)
        {
            var slip = await _sender.Send(new GetPayslipQuery(salaryId), HttpContext.RequestAborted);
            if (slip == null) return NotFound();
            return File(slip.Content, slip.ContentType, slip.FileName);
        }

    }
}
