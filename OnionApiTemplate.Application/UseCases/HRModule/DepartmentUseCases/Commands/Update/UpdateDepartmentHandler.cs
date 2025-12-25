using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Update
{
    internal class UpdateDepartmentHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateDepartmentCommand> validator,
        UserManager<ApplicationUser> userManager,
        ILogger<UpdateDepartmentHandler> logger
    ) : IRequestHandler<UpdateDepartmentCommand, DepartmentDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateDepartmentCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<UpdateDepartmentHandler> _logger = logger;

        public async Task<DepartmentDetailsDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update department ID: {Id}", request.Id);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var repository = _unitOfWork.GetRepository<Department, Guid>();
            var existingDepartment = await repository.GetByIdAsync(request.Id, cancellationToken);

            if (existingDepartment is null)
            {
                _logger.LogWarning("Update failed: Department {Id} not found", request.Id);
                throw new NotFoundException<Department>(request.Id);
            }

            if (existingDepartment.IsDeleted)
            {
                _logger.LogWarning("Update failed: Department {Id} is soft-deleted", request.Id);
                throw new BadRequestException("Cannot update a deleted department.");
            }

            if (existingDepartment.Name != request.Dto.Name)
            {
                var conflictingDept = await repository.FirstOrDefaultAsync(d =>
                    d.Name == request.Dto.Name &&
                    d.Id != request.Id, cancellationToken);

                if (conflictingDept is not null)
                {
                    if (conflictingDept.IsDeleted)
                    {
                        _logger.LogWarning("Conflict: Department '{Name}' exists but is deleted.", request.Dto.Name);
                        throw new BadRequestException($"A department named '{request.Dto.Name}' already exists in the archives (deleted). Please toggle/restore it instead of renaming this one.");
                    }

                    _logger.LogWarning("Update conflict: Name '{Name}' is currently in use by another active department.", request.Dto.Name);
                    throw new AlreadyExistsException<Department>(request.Dto.Name);
                }
            }

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogWarning("Update failed: User {UserId} not found", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            _mapper.Map(request.Dto, existingDepartment);
            existingDepartment.ModifiedAt = DateTime.UtcNow;
            existingDepartment.ModifiedBy = request.CurrentUserId;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Department {Id} updated successfully by {UserId}", request.Id, request.CurrentUserId);

            return _mapper.Map<DepartmentDetailsDto>(existingDepartment);
        }
    }
}