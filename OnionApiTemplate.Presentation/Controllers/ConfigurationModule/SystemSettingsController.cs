using Khazen.Application.Common.QueryParameters;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Create;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Delete;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateByKey;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateValueByKey;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Queries.GetAll;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Queries.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Khazen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISender _sender;

        public SystemSettingsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SystemSettingsQueryParameters parameters)
        {
            var result = await _sender.Send(new GetAllSystemSettingsQuery(parameters));
            return Ok(result);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var result = await _sender.Send(new GetSystemSettingByKeyQuery(key));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSystemSettingCommand command)
        {
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetByKey), new { key = result.Key }, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSystemSettingCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPatch("{key}/value")]
        public async Task<IActionResult> UpdateValue(string key, [FromBody] UpdateSystemSettingValueCommand command)
        {
            if (key != command.Key) return BadRequest("Key mismatch");

            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            return Ok(await _sender.Send(new ToggleSystemSettingCommand(key)));
        }
    }
}
