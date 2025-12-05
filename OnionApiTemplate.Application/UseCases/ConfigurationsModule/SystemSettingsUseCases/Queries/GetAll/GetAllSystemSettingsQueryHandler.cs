using Khazen.Application.Common;
using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;
using Khazen.Application.Specification.ConfigurationModule.SystemSettingSpecifications;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Queries.GetAll
{
    internal class GetAllSystemSettingsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetAllSystemSettingsQuery> validator,
        ILogger<GetAllSystemSettingsQueryHandler> logger)
        : IRequestHandler<GetAllSystemSettingsQuery, PaginatedResult<SystemSettingDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllSystemSettingsQuery> _validator = validator;
        private readonly ILogger<GetAllSystemSettingsQueryHandler> _logger = logger;

        public async Task<PaginatedResult<SystemSettingDto>> Handle(GetAllSystemSettingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start GetAllSystemSettingsQueryHandler | Page: {PageIndex}, Size: {PageSize}", request.Parameters.PageIndex, request.Parameters.PageSize);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for GetAllSystemSettingsQuery: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var systemSettingsRepository = _unitOfWork.GetRepository<SystemSetting, int>();
                var systemSettings = await systemSettingsRepository.GetAllAsync(
                    new GetAllSystemSettingsSpec(request.Parameters),
                    cancellationToken,
                    true);

                var count = await systemSettingsRepository.GetCountAsync(
                    new GetAllSystemSettingsCountSpec(request.Parameters),
                    cancellationToken);

                var systemSettingsDto = _mapper.Map<List<SystemSettingDto>>(systemSettings);

                _logger.LogInformation("Retrieved {Count} SystemSettings (Page {PageIndex}) successfully.", count, request.Parameters.PageIndex);

                return new PaginatedResult<SystemSettingDto>(request.Parameters.PageIndex, request.Parameters.PageSize, count, systemSettingsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving SystemSettings (Page {PageIndex})", request.Parameters.PageIndex);
                throw;
            }
        }
    }
}
