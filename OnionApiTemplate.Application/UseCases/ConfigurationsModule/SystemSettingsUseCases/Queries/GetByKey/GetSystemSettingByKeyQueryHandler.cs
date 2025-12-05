using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;
using Khazen.Application.Specification.ConfigurationModule.SystemSettingSpecifications;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Queries.GetByKey
{
    internal class GetSystemSettingByKeyQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetSystemSettingByKeyQuery> validator,
        ILogger<GetSystemSettingByKeyQueryHandler> logger)
        : IRequestHandler<GetSystemSettingByKeyQuery, SystemSettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetSystemSettingByKeyQuery> _validator = validator;
        private readonly ILogger<GetSystemSettingByKeyQueryHandler> _logger = logger;

        public async Task<SystemSettingDto> Handle(GetSystemSettingByKeyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start GetSystemSettingByKeyQueryHandler for Key: {Key}", request.Key);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for GetSystemSettingByKeyQuery: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var systemSettingsRepository = _unitOfWork.GetRepository<SystemSetting, int>();
                var systemSetting = await systemSettingsRepository.GetAsync(new GetSystemSettingByKeySpec(request.Key), cancellationToken, true);

                if (systemSetting == null)
                {
                    _logger.LogWarning("SystemSetting with Key '{Key}' not found.", request.Key);
                    throw new NotFoundException<SystemSetting>(request.Key);
                }

                _logger.LogInformation("SystemSetting with Key '{Key}' retrieved successfully.", request.Key);
                return _mapper.Map<SystemSettingDto>(systemSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving SystemSetting with Key '{Key}'", request.Key);
                throw;
            }
        }
    }
}
