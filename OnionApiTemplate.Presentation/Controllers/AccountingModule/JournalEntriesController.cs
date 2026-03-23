using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Create;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Delete;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetAll;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.AccountingModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JournalEntriesController(ISender mediator) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPost]
        [CacheInvalidate("/api/JournalEntries")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<ActionResult<JournalEntryDetailsDto>> Create([FromBody] CreateJournalEntryDto dto)
        {
            var result = await mediator.Send(new CreateJournalEntryCommand(dto, CurrentUserId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        [CacheInvalidate("/api/JournalEntries")]
        [CacheInvalidate("/api/Accounts")]
        public async Task<ActionResult<bool>> Reverse(Guid id, [FromHeader(Name = "If-Match")] string rowVersion)
        {
            var rowVersionBytes = Convert.FromBase64String(rowVersion);
            var result = await mediator.Send(new ReverseJournalEntryCommand(id, rowVersionBytes, CurrentUserId));

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [RedisCache(300)]
        public async Task<ActionResult<JournalEntryDetailsDto>> GetById(Guid id)
        {
            var result = await mediator.Send(new GetJournalEntryByIdQuery(id));
            return Ok(result);
        }

        [HttpGet]
        [RedisCache(60)]
        public async Task<ActionResult<IEnumerable<JournalEntryDto>>> GetAll([FromQuery] JurnalEntryQueryParameters queryParameters)
        {
            var result = await mediator.Send(new GetPaginatedJournalEntriesQuery(queryParameters));
            return Ok(result);
        }
    }
}