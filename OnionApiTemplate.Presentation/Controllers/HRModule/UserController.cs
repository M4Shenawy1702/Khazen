using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Toggle;
using Khazen.Presentation.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Khazen.Presentation.Controllers.HRModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, HRManager")]
    public class UserController(ISender sender) : ControllerBase
    {
        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpPut("toggle/{id}")]
        [CacheInvalidate("/api/User")]
        [CacheInvalidate("/api/Employee")]
        public async Task<IActionResult> ToggleUserById(string id)
        {

            var result = await sender.Send(new ToggleUserCommand(id, CurrentUserId));

            return Ok(result);
        }
    }
}