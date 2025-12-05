using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.BonusDtos;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Add;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Delete;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonusesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BonusesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<BonusDto>> AddBonus(AddBonusDto dto)
        {
            var userName = User.Identity?.Name ?? "System";
            var result = await _mediator.Send(new AddBonusCommand(dto, userName));
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<BonusDto>>> GetAllBonuses(BonusQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetAllBounsQuery(queryParameters));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBonus(int id)
        {
            var userName = User.Identity?.Name;
            if (userName == null) return BadRequest("User name is null");
            var result = await _mediator.Send(new ToggleBonusCommand(id, userName));
            return Ok(result);
        }
    }
}
