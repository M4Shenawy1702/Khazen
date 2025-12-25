using Khazen.Application.DOTs.CongifurationModule.CompanySetting;
using Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.UpdateThemeColor;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.CompanySettingUsecases.Commands.UpdateThemeColor
{
    internal class UpdateThemeColorCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateThemeColorCommand> validator,
        ILogger<UpdateThemeColorCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<UpdateThemeColorCommand, CompanySettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateThemeColorCommand> _validator = validator;
        private readonly ILogger<UpdateThemeColorCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<CompanySettingDto> Handle(UpdateThemeColorCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start UpdateThemeColorCommandHandler for color: {ThemeColor}", request.ThemeColor);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for theme color update: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var companyRepository = _unitOfWork.GetRepository<CompanySetting, int>();

            var companySetting = await companyRepository.FirstOrDefaultAsync(cancellationToken);

            if (companySetting == null)
            {
                _logger.LogError("CompanySetting record not found. Cannot update theme color.");
                throw new NotFoundException<CompanySetting>("Global Company Settings record not found.");
            }

            if (companySetting.ThemeColor == request.ThemeColor)
            {
                _logger.LogInformation("Theme color is already set to the same value: {ThemeColor}. No update necessary.", request.ThemeColor);
                return _mapper.Map<CompanySettingDto>(companySetting);
            }

            companySetting.ThemeColor = request.ThemeColor;
            companySetting.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("CompanySetting theme color updated successfully to {ThemeColor} .",
                request.ThemeColor);

            return _mapper.Map<CompanySettingDto>(companySetting);
        }
    }
}