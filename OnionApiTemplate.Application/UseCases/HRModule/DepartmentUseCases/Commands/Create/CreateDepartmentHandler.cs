using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Create;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

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
        _logger.LogDebug("Starting CreateDepartmentHandler for DepartmentName: {DepartmentName}, CreatedBy: {CreatedBy}",
            request.Dto.Name, request.CreatedBy);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreateDepartmentCommand: {Errors}",
                string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var departmentRepo = _unitOfWork.GetRepository<Department, Guid>();
            var exists = await departmentRepo.AnyAsync(d => d.Name == request.Dto.Name && !d.IsDeleted, cancellationToken);
            if (exists)
            {
                _logger.LogWarning("Department already exists with Name: {DepartmentName}", request.Dto.Name);
                throw new AlreadyExistsException<Department>(request.Dto.Name);
            }

            var user = await _userManager.FindByNameAsync(request.CreatedBy);
            if (user is null)
            {
                _logger.LogWarning("User not found. CreatedBy: {CreatedBy}", request.CreatedBy);
                throw new NotFoundException<ApplicationUser>(request.CreatedBy);
            }

            var department = _mapper.Map<Department>(request.Dto);
            department.CreatedBy = request.CreatedBy;

            await departmentRepo.AddAsync(department, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Department created successfully. DepartmentId: {DepartmentId}, Name: {DepartmentName}, CreatedBy: {CreatedBy}",
                department.Id, department.Name, request.CreatedBy);

            return _mapper.Map<DepartmentDetailsDto>(department);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating department: {DepartmentName}", request.Dto.Name);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw new ApplicationException("An unexpected error occurred while creating the department.", ex);
        }
    }
}
