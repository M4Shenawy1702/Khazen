using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;
using Khazen.Application.Specification.ConfigurationModule.SystemSettingSpecifications;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateByKey
{
    internal class UpdateSystemSettingCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateSystemSettingCommand> validator,
        ILogger<UpdateSystemSettingCommandHandler> logger)
        : IRequestHandler<UpdateSystemSettingCommand, SystemSettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateSystemSettingCommand> _validator = validator;
        private readonly ILogger<UpdateSystemSettingCommandHandler> _logger = logger;

        public async Task<SystemSettingDto> Handle(UpdateSystemSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start UpdateSystemSettingCommandHandler for Id: {Id}, Key: {Key}", request.Id, request.Dto.Key);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for UpdateSystemSettingCommand: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var systemSettingsRepository = _unitOfWork.GetRepository<SystemSetting, int>();

                _logger.LogDebug("Retrieving SystemSetting with Id {Id}...", request.Id);
                var systemSetting = await systemSettingsRepository.GetAsync(
                    new GetSystemSettingByIdSpec(request.Id),
                    cancellationToken
                ) ?? throw new NotFoundException<SystemSetting>(request.Id);

                _logger.LogDebug("Checking for duplicate SystemSetting Key '{Key}'...", request.Dto.Key);
                var existingSystemSetting = await systemSettingsRepository.GetAsync(
                    new GetSystemSettingByKeySpec(request.Dto.Key),
                    cancellationToken
                );

                if (existingSystemSetting is not null && existingSystemSetting.Id != systemSetting.Id)
                {
                    _logger.LogError("Duplicate Key '{Key}' found for another SystemSetting.", request.Dto.Key);
                    throw new AlreadyExistsException<SystemSetting>(request.Dto.Key);
                }

                _mapper.Map(request.Dto, systemSetting);
                systemSettingsRepository.Update(systemSetting);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("SystemSetting with Id {Id} updated successfully.", request.Id);
                return _mapper.Map<SystemSettingDto>(systemSetting);
            }
            catch (NotFoundException)
            {
                _logger.LogWarning("SystemSetting with Id {Id} not found.", request.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating SystemSetting with Id {Id}.", request.Id);
                throw;
            }
        }
    }
}
