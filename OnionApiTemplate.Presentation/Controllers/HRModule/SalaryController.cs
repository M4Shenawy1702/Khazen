using Khazen.Application.Common.QueryParameters;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetById;
using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetPayslip;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly ISender _sender;

        public SalaryController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{salaryId:guid}")]
        public async Task<IActionResult> GetSalaryById(Guid salaryId)
        {
            var dto = await _sender.Send(new GetSalaryByIdQuery(salaryId));
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalaries([FromQuery] SalariesQueryParameters queryParameters)
        {
            var query = new GetSalariesQuery(queryParameters);
            var paged = await _sender.Send(query);
            return Ok(paged);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalary([FromBody] CreateSalaryCommand command)
        {
            var created = await _sender.Send(command);
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

        //// ----- Payroll & Calculation -----

        [HttpGet("{salaryId:guid}/payslip")]
        public async Task<IActionResult> GetPayslip(Guid salaryId)
        {
            var slip = await _sender.Send(new GetPayslipQuery(salaryId), HttpContext.RequestAborted);
            if (slip == null) return NotFound();
            return File(slip.Content, slip.ContentType, slip.FileName);
        }


        //// ----- History & Audit -----

        //[HttpGet("{salaryId:guid}/history")]
        //public async Task<IActionResult> GetSalaryHistory(Guid salaryId)
        //{
        //    var history = await _sender.Send(new GetSalaryHistoryQuery(salaryId));
        //    return Ok(history);
        //}

        //[HttpGet("employee/{employeeId:guid}/summary")]
        //public async Task<IActionResult> GetEmployeeSalarySummary(Guid employeeId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        //{
        //    var summary = await _sender.Send(new GetEmployeeSalarySummaryQuery(employeeId, from, to));
        //    return Ok(summary);
        //}

        //// ----- Imports / Exports -----

        //[HttpGet("export")]
        //public async Task<IActionResult> ExportPayslips([FromQuery] string period, [FromQuery] string format)
        //{
        //    var export = await _sender.Send(new ExportPayslipsQuery(period, format));
        //    return File(export.Content, export.ContentType, export.FileName);
        //}

        //[HttpPost("import")]
        //public async Task<IActionResult> ImportSalaries([FromForm] ImportSalariesCommand command)
        //{
        //    var result = await _sender.Send(command);
        //    return Ok(result);
        //}
    }
}
