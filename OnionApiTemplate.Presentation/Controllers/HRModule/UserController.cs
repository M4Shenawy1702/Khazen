using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Toggle;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(ISender sender)
        : ControllerBase
    {
        private readonly ISender _sender = sender;
        [HttpPut("ToogleUserById/{id}")]
        public async Task<IActionResult> ToggleUserById(string id)
        {
            var query = new ToggleUserCommand(id);
            var result = await _sender.Send(query);
            return Ok(result);
        }
    }
}
