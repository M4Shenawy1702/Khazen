using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Create
{
    internal class CreateDepartmentHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateDepartmentCommand> validator,
        UserManager<ApplicationUser> userManager,
        ILogger<CreateDepartmentHandler> logger
    ) : IRequestHandler<CreateDepartmentCommand, DepartmentDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateDepartmentCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<CreateDepartmentHandler> _logger = logger;

        public async Task<DepartmentDetailsDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create department: {DepartmentName}", request.Dto.Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for {DepartmentName}", request.Dto.Name);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var departmentRepo = _unitOfWork.GetRepository<Department, Guid>();

                var exists = await departmentRepo.AnyAsync(d => d.Name == request.Dto.Name && !d.IsDeleted, cancellationToken);
                if (exists)
                {
                    _logger.LogWarning("Conflict: {DepartmentName} already exists", request.Dto.Name);
                    throw new AlreadyExistsException<Department>(request.Dto.Name);
                }

                var user = await _userManager.FindByNameAsync(request.CurrentUserId);
                if (user is null)
                {
                    _logger.LogError("User {UserId} not found", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                var department = _mapper.Map<Department>(request.Dto);
                department.CreatedBy = request.CurrentUserId;
                department.CreatedAt = DateTime.UtcNow;

                await departmentRepo.AddAsync(department, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Department {Name} created (ID: {Id})", department.Name, department.Id);

                return _mapper.Map<DepartmentDetailsDto>(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create department {Name}", request.Dto.Name);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}