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
        ILogger<UpdateThemeColorCommandHandler> logger)
        : IRequestHandler<UpdateThemeColorCommand, CompanySettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateThemeColorCommand> _validator = validator;
        private readonly ILogger<UpdateThemeColorCommandHandler> _logger = logger;

        public async Task<CompanySettingDto> Handle(UpdateThemeColorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start UpdateThemeColorCommandHandler");

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for UpdateThemeColorCommandHandler: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var companyRepository = _unitOfWork.GetRepository<CompanySetting, int>();
                var companySetting = await companyRepository.SingleOrDefaultAsync(cancellationToken);

                if (companySetting == null)
                {
                    _logger.LogError("CompanySetting not found, Add a CompanySetting first");
                    throw new NotFoundException<CompanySetting>("CompanySetting not found, Add a CompanySetting first");
                }

                if (companySetting.ThemeColor == request.ThemeColor)
                {
                    _logger.LogInformation("Theme color is already set to the same value");
                    return _mapper.Map<CompanySettingDto>(companySetting);
                }

                companySetting.ThemeColor = request.ThemeColor;

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("CompanySetting theme color updated successfully");

                return _mapper.Map<CompanySettingDto>(companySetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error happened while updating CompanySetting theme color");
                throw;
            }
        }
    }
}
