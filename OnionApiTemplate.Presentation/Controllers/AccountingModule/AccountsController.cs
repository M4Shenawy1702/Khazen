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
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController(ISender sender) : ControllerBase
{
    private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                         ?? throw new UnauthorizedAccessException("User identity not available.");

    [HttpPost]
    [CacheInvalidate("/api/Accounts")]
    public async Task<ActionResult<AccountDetailsDto>> Create([FromBody] CreateAccountDto dto)
    {
        var result = await sender.Send(new CreateAccountCommand(dto, CurrentUserId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [CacheInvalidate("/api/Accounts")]
    public async Task<ActionResult<AccountDetailsDto>> Update(Guid id, [FromBody] UpdateAccountDto dto, [FromHeader(Name = "If-Match")] string rowVersion)
    {
        var rowVersionBytes = Convert.FromBase64String(rowVersion);
        var result = await sender.Send(new UpdateAccountByIdCommand(id, dto, rowVersionBytes, CurrentUserId));
        return Ok(result);
    }

    [HttpPatch("toggle/{id:guid}")]
    [CacheInvalidate("/api/Accounts")]
    public async Task<ActionResult<bool>> Toggle(Guid id, [FromHeader(Name = "If-Match")] string rowVersion)
    {
        var rowVersionBytes = Convert.FromBase64String(rowVersion);
        var result = await sender.Send(new ToggleAccountByIdCommand(id, rowVersionBytes, CurrentUserId));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RedisCache(360)]
    public async Task<ActionResult<AccountDetailsDto>> GetById(Guid id)
    {
        var result = await sender.Send(new GetAccountByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
    [RedisCache(360)]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll([FromQuery] AccountQueryParameters queryParameters)
    {
        var result = await sender.Send(new GetAllAccountsQuery(queryParameters));
        return Ok(result);
    }
}