using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Create;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Delete;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetAll;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.AccountingModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalEntriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JournalEntriesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<JournalEntryDetailsDto>> Create(CreateJournalEntryDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new CreateJournalEntryCommand(dto, user));

            return CreatedAtRoute("GetJournalEntryById", new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> Reverse(Guid id, [FromHeader] byte[] rowVersion)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _mediator.Send(new ReverseJournalEntryCommand(id, rowVersion, user));
            return result ? NoContent() : NotFound();
        }

        [HttpGet("{id:guid}", Name = "GetJournalEntryById")]
        public async Task<ActionResult<JournalEntryDetailsDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetJournalEntryByIdQuery(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JournalEntryDto>>> GetAll([FromQuery] JurnalEntryQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetPaginatedJournalEntriesQuery(queryParameters));
            return Ok(result);
        }
    }
}