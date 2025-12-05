using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.AdvanceDtos;
using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Create;
using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Toggle;
using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvancesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdvancesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<AdvanceDto>> AddAdvance(AddAdvanceDto dto)
        {
            var userName = User.Identity?.Name;
            if (userName == null) return BadRequest("User name is null");
            var result = await _mediator.Send(new AddAdvanceCommand(dto, userName));
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<AdvanceDto>>> GetAllAdvances([FromQuery] AdvanceQueryParameters QueryParameters)
        {
            var result = await _mediator.Send(new GetAllAdvanceQuery(QueryParameters));
            return Ok(result);
        }



        [HttpPatch("{id}")]
        public async Task<IActionResult> ToggleAdvance(int id)
        {
            var userName = User.Identity?.Name;
            if (userName == null) return BadRequest("User name is null");
            return Ok(_mediator.Send(new ToggleAdvanceCommand(id, userName)));
        }
    }
}
