using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.SalaryDots;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetById;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetPayslip;
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
    public class SalaryController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                   ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet("{salaryId:guid}")]
        [RedisCache(300)]
        public async Task<IActionResult> GetSalaryById(Guid salaryId)
        {
            var result = await sender.Send(new GetSalaryByIdQuery(salaryId));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet]
        [RedisCache(120)]
        public async Task<IActionResult> GetSalaries([FromQuery] SalariesQueryParameters queryParameters)
        {
            var paged = await sender.Send(new GetAllSalariesQuery(queryParameters));
            return Ok(paged);
        }

        [HttpPost]
        [CacheInvalidate("/api/Salary")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<IActionResult> CreateSalary([FromBody] CreateSalaryDto dto)
        {
            var created = await sender.Send(new CreateSalaryCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetSalaryById), new { salaryId = created.Id }, created);
        }

        [HttpDelete("{salaryId:guid}")]
        [CacheInvalidate("/api/Salary")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<IActionResult> DeleteSalary(Guid salaryId)
        {
            await sender.Send(new DeleteSalaryCommand(salaryId, CurrentUserId));
            return NoContent();
        }

        [HttpGet("{salaryId:guid}/payslip")]
        public async Task<IActionResult> GetPayslip(Guid salaryId)
        {
            var slip = await sender.Send(new GetPayslipQuery(salaryId), HttpContext.RequestAborted);
            if (slip == null) return NotFound();

            return File(slip.Content, slip.ContentType, slip.FileName);
        }
    }
}