using Khazen.Application.DOTs.CongifurationModule.CompanySetting;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.Update
{
    internal class UpdateCompanySettingsCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateCompanySettingsCommand> validator,
        ILogger<UpdateCompanySettingsCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<UpdateCompanySettingsCommand, CompanySettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateCompanySettingsCommand> _validator = validator;
        private readonly ILogger<UpdateCompanySettingsCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<CompanySettingDto> Handle(UpdateCompanySettingsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start UpdateCompanySettingsCommandHandler for configuration data.");

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for company settings update: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var updaterUser = await _userManager.FindByNameAsync(request.ModifiedBy);
            if (updaterUser == null)
            {
                _logger.LogWarning("Settings update failed: Updater user '{Username}' not found.", request.ModifiedBy);
                throw new NotFoundException($"Updater user '{request.ModifiedBy}' not found.");
            }

            var companyRepository = _unitOfWork.GetRepository<CompanySetting, int>();

            var companySetting = await companyRepository.FirstOrDefaultAsync(cancellationToken);
            if (companySetting == null)
            {
                _logger.LogError("CompanySetting record not found. Database must contain a settings record.");
                throw new NotFoundException<CompanySetting>("Global Company Settings record not found.");
            }

            _logger.LogDebug("Applying updates and auditing fields to CompanySetting ID: {SettingId}", companySetting.Id);

            _mapper.Map(request.Dto, companySetting);

            companySetting.ModifiedBy = updaterUser.Id;
            companySetting.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("CompanySetting ID {SettingId} updated successfully by User ID: {UserId}.",
                companySetting.Id, updaterUser.Id);

            return _mapper.Map<CompanySettingDto>(companySetting);
        }
    }
}