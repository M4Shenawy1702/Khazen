using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Create
{
    internal class CreateSystemSettingCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateSystemSettingCommand> validator,
        ILogger<CreateSystemSettingCommandHandler> logger)
        : IRequestHandler<CreateSystemSettingCommand, SystemSettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateSystemSettingCommand> _validator = validator;
        private readonly ILogger<CreateSystemSettingCommandHandler> _logger = logger;

        public async Task<SystemSettingDto> Handle(CreateSystemSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start CreateSystemSettingCommandHandler for Key: {Key}", request.Dto.Key);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for CreateSystemSettingCommand: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }
                _logger.LogDebug("Validation passed for CreateSystemSettingCommand.");

                var systemSettingsRepository = _unitOfWork.GetRepository<SystemSetting, int>();

                _logger.LogDebug("Checking if SystemSetting with Key '{Key}' already exists...", request.Dto.Key);
                var existingSystemSetting = await systemSettingsRepository.AnyAsync(s => s.Key == request.Dto.Key, cancellationToken);

                if (existingSystemSetting)
                {
                    _logger.LogError("SystemSetting with Key '{Key}' already exists", request.Dto.Key);
                    throw new AlreadyExistsException<SystemSetting>(request.Dto.Key);
                }

                var systemSetting = _mapper.Map<SystemSetting>(request.Dto);
                await systemSettingsRepository.AddAsync(systemSetting, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("SystemSetting with Key '{Key}' created successfully.", request.Dto.Key);

                return _mapper.Map<SystemSettingDto>(systemSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating SystemSetting with Key '{Key}'", request.Dto.Key);
                throw;
            }
        }
    }
}
