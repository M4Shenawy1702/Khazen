using Khazen.Application.Specification.ConfigurationModule.SystemSettingSpecifications;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Delete;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Toggle
{
    internal class ToggleSystemSettingCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<ToggleSystemSettingCommand> validator,
        ILogger<ToggleSystemSettingCommandHandler> logger)
        : IRequestHandler<ToggleSystemSettingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<ToggleSystemSettingCommand> _validator = validator;
        private readonly ILogger<ToggleSystemSettingCommandHandler> _logger = logger;

        public async Task<bool> Handle(ToggleSystemSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start ToggleSystemSettingCommandHandler for Key: {Key}", request.Key);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for ToggleSystemSettingCommand: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var systemSettingsRepository = _unitOfWork.GetRepository<SystemSetting, int>();
                var systemSetting = await systemSettingsRepository.GetAsync(new GetSystemSettingByKeySpec(request.Key), cancellationToken);

                if (systemSetting == null)
                {
                    _logger.LogWarning("SystemSetting with Key '{Key}' not found.", request.Key);
                    throw new NotFoundException<SystemSetting>(request.Key);
                }

                systemSetting.IsDeleted = !systemSetting.IsDeleted;
                _logger.LogDebug("Toggling IsDeleted status for SystemSetting '{Key}' to {IsDeleted}", request.Key, systemSetting.IsDeleted);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("SystemSetting '{Key}' toggled successfully. New IsDeleted = {IsDeleted}", request.Key, systemSetting.IsDeleted);

                return systemSetting.IsDeleted;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling SystemSetting with Key '{Key}'", request.Key);
                throw;
            }
        }
    }
}
