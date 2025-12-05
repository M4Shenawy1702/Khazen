using Khazen.Application.Common.QueryParameters;
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
        public async Task<IActionResult> Create(CreateJournalEntryCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new ReverseJournalEntryCommand(id));
            return result ? NoContent() : NotFound();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetJournalEntryByIdQuery(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] JurnalEntryQueryParameters queryParameters)
        {
            var result = await _mediator.Send(new GetPaginatedJournalEntriesQuery(queryParameters));
            return Ok(result);
        }
    }

}
