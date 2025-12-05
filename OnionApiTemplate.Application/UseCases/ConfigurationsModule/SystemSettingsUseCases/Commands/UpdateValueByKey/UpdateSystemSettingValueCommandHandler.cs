using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;
using Khazen.Application.Specification.ConfigurationModule.SystemSettingSpecifications;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateValueByKey
{
    internal class UpdateSystemSettingValueCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateSystemSettingValueCommand> validator)
        : IRequestHandler<UpdateSystemSettingValueCommand, SystemSettingDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateSystemSettingValueCommand> _validator = validator;
        public async Task<SystemSettingDto> Handle(UpdateSystemSettingValueCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

            var systemSettingsRepository = _unitOfWork.GetRepository<SystemSetting, int>();

            var systemSetting = await systemSettingsRepository.GetAsync(new GetSystemSettingByKeySpec(request.Key), cancellationToken) ??
                throw new NotFoundException<SystemSetting>(request.Key);

            _mapper.Map(request, systemSetting);

            systemSettingsRepository.Update(systemSetting);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SystemSettingDto>(systemSetting);
        }
    }
}
