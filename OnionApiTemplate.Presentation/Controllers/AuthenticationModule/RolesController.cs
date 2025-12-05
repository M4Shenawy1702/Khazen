using Khazen.Application.UseCases.AuthService.RolesModule.Commands.AddRole;
using Khazen.Application.UseCases.AuthService.RolesModule.Commands.AssignRole;
using Khazen.Application.UseCases.AuthService.RolesModule.Commands.DeleteRole;
using Khazen.Application.UseCases.AuthService.RolesModule.Commands.RevokeRole;
using Khazen.Application.UseCases.AuthService.RolesModule.Commands.UpdateRole;
using Khazen.Application.UseCases.AuthService.RolesModule.Queries.GetAllRoles;
using Khazen.Application.UseCases.AuthService.RolesModule.Queries.GetRoleById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.AuthenticationModule
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public class RolesController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var result = await _sender.Send(new DeleteRoleCommand(roleId));
            return Ok(result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeRole([FromBody] RevokeRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _sender.Send(new GetAllRolesQuery());
            return Ok(result);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var result = await _sender.Send(new GetRoleByIdQuery(roleId));
            return Ok(result);
        }
    }
}
