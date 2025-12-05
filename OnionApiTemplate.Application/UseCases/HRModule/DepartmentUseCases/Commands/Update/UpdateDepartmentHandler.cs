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
            _logger.LogDebug("Starting UpdateDepartmentHandler for DepartmentId: {DepartmentId}, ModifiedBy: {ModifiedBy}",
                request.Id, request.ModifiedBy);

            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for UpdateDepartmentCommand: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var repository = _unitOfWork.GetRepository<Department, Guid>();
                var existingDepartment = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (existingDepartment is null)
                {
                    _logger.LogWarning("Department not found. DepartmentId: {DepartmentId}", request.Id);
                    throw new NotFoundException<Department>(request.Id);
                }

                if (existingDepartment.IsDeleted)
                {
                    _logger.LogWarning("Cannot update deleted department. DepartmentId: {DepartmentId}", request.Id);
                    throw new BadRequestException($"Department with ID '{request.Id}' is deleted and cannot be updated.");
                }

                var nameExists = await repository.AnyAsync(d => d.Name == request.Dto.Name && d.Id != request.Id, cancellationToken);
                if (nameExists)
                {
                    _logger.LogWarning("Department name conflict. Name: {DepartmentName}", request.Dto.Name);
                    throw new AlreadyExistsException<Department>(request.Dto.Name);
                }

                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogWarning("User not found. UserId: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }

                _mapper.Map(request.Dto, existingDepartment);
                existingDepartment.ModifiedAt = DateTime.UtcNow;
                existingDepartment.ModifiedBy = request.ModifiedBy;

                repository.Update(existingDepartment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Department updated successfully. DepartmentId: {DepartmentId}, ModifiedBy: {ModifiedBy}",
                    existingDepartment.Id, request.ModifiedBy);

                return _mapper.Map<DepartmentDetailsDto>(existingDepartment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department. DepartmentId: {DepartmentId}", request.Id);
                throw new ApplicationException("An unexpected error occurred while updating the department.", ex);
            }
        }
    }
}
