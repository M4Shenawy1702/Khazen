using Khazen.Application.DOTs.CongifurationModule.CompanySetting;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.Update
{
    internal class UpdateCompanySettingsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateCompanySettingsCommand> validator, ILogger<UpdateCompanySettingsCommandHandler> logger)
        : IRequestHandler<UpdateCompanySettingsCommand, CompanySettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateCompanySettingsCommand> _validator = validator;
        private readonly ILogger<UpdateCompanySettingsCommandHandler> _logger = logger;

        public async Task<CompanySettingDto> Handle(UpdateCompanySettingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start UpdateCompanySettingsCommandHandler");
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for UpdateCompanySettingsCommandHandler : {Errors}",
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

                _mapper.Map(request.Dto, companySetting);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("CompanySetting updated Successfully");
                return _mapper.Map<CompanySettingDto>(companySetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error happen while updating CompanySetting");
                throw;
            }
        }
    }
}