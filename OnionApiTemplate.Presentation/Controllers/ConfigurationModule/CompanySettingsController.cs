using Khazen.Application.DOTs.CongifurationModule.CompanySetting;
using Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.Update;
using Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.UpdateThemeColor;
using Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.Presentation.Controllers.ConfigurationModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanySettingsController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        private string CurrentUserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
         ?? throw new UnauthorizedAccessException("User identity not available.");

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var result = await _sender.Send(new GetCompanySettingsQuery());
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCompanySettingsDto Dto)
        {
            var result = await _sender.Send(new UpdateCompanySettingsCommand(Dto, CurrentUserId));
            return Ok(result);
        }

        [HttpPatch("theme-color")]
        public async Task<IActionResult> UpdateThemeColor([FromBody] string ThemeColor)
        {
            var result = await _sender.Send(new UpdateThemeColorCommand(ThemeColor, CurrentUserId));
            return Ok(result);
        }
    }
}
