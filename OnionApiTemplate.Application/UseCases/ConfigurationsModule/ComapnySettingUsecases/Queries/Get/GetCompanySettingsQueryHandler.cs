using Khazen.Application.DOTs.CongifurationModule.CompanySetting;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Queries.Get
{
    internal class GetCompanySettingsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetCompanySettingsQueryHandler> logger)
        : IRequestHandler<GetCompanySettingsQuery, CompanySettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetCompanySettingsQueryHandler> _logger = logger;

        public async Task<CompanySettingDto> Handle(GetCompanySettingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start GetCompanySettingsQueryHandler");

                var companyRepo = _unitOfWork.GetRepository<CompanySetting, int>();
                var companySettings = await companyRepo.SingleOrDefaultAsync(cancellationToken);

                if (companySettings == null)
                {
                    _logger.LogError("CompanySetting not found, Add a CompanySetting first");
                    throw new NotFoundException<CompanySetting>("CompanySetting not found, Add a CompanySetting first");
                }

                _logger.LogInformation("CompanySettings retrieved successfully");
                return _mapper.Map<CompanySettingDto>(companySettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error happened while fetching data in GetCompanySettingsQueryHandler");
                throw;
            }
        }
    }
}
