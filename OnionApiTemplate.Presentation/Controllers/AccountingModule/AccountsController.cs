using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.AccountingModule.AccountDtos;
using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Create;
using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Delete;
using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Update;
using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Queries.GetAll;
using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Queries.GetById;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.AccountingModule
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly ISender _sender;

        public AccountsController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost]
        public async Task<ActionResult<AccountDetailsDto>> Create([FromBody] CreateAccountDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _sender.Send(new CreateAccountCommand(dto, user));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<AccountDetailsDto>> Update(Guid id, [FromBody] UpdateAccountDto dto)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _sender.Send(new UpdateAccountByIdCommand(id, dto, user));
            return Ok(result);
        }

        [HttpPatch("toggle/{id:guid}")]
        public async Task<ActionResult<bool>> Toggle(Guid id)
        {
            var user = User.Identity?.Name;
            if (user == null)
                return BadRequest("User not found");
            var result = await _sender.Send(new ToggleAccountByIdCommand(id, user));
            return result ? NoContent() : NotFound();
        }

        [HttpGet("{id:guid}")]
        [RedisCache(360)]
        public async Task<ActionResult<AccountDetailsDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetAccountByIdQuery(id));
            return Ok(result);
        }

        [HttpGet]
        [RedisCache(360)]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll([FromQuery] AccountQueryParameters queryParameters)
        {
            var result = await _sender.Send(new GetAllAccountsQuery(queryParameters));
            return Ok(result);
        }
    }
}
