using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.DeductionDtos;
using Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Add;
using Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetAll;
using Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeductionsController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DeductionQueryParameters queryParameters)
        {
            var result = await _sender.Send(new GetAllDeductionsQuery(queryParameters));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetDeductionByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddDeductionDto dto)
        {
            var result = await _sender.Send(new AddDeductionCommand(dto, User.Identity?.Name!));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sender.Send(new ToggleDeductionCommand(id, User.Identity?.Name!));
            return NoContent();
        }
    }
}
