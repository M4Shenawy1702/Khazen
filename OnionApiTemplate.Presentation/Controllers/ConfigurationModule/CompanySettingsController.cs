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

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var result = await _sender.Send(new GetCompanySettingsQuery());
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCompanySettingsCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPatch("theme-color")]
        public async Task<IActionResult> UpdateThemeColor([FromBody] UpdateThemeColorCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
    }
}
