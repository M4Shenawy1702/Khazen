using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.CreateEmployee
{
    internal class CreateUserHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IValidator<CreateEmployeeCommand> validator,
        ILogger<CreateUserHandler> logger
    ) : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IValidator<CreateEmployeeCommand> _validator = validator;
        private readonly ILogger<CreateUserHandler> _logger = logger;

        public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting CreateUserHandler for Email: {Email}, Username: {Username}, DepartmentId: {DepartmentId}, CreatedBy: {CreatedBy}",
                request.Dto.Email, request.Dto.UserName, request.Dto.DepartmentId, request.CreatedBy);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var creatorUser = await _userManager.FindByNameAsync(request.CreatedBy);
            if (creatorUser == null)
            {
                _logger.LogWarning("User not found: {CreatedBy}", request.CreatedBy);
                throw new NotFoundException<ApplicationUser>(request.CreatedBy);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            ApplicationUser newUser = null!;
            try
            {
                var departmentRepo = _unitOfWork.GetRepository<Department, Guid>();
                var department = await departmentRepo.GetByIdAsync(request.Dto.DepartmentId, cancellationToken)
                    ?? throw new NotFoundException<Department>(request.Dto.DepartmentId);

                if (!department.IsDeleted)
                {
                    _logger.LogWarning("Cannot create employee in inactive department: {DepartmentId}", request.Dto.DepartmentId);
                    throw new BadRequestException("Cannot create an employee in an inactive department. Please activate it first.");
                }

                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();

                var nationalIdTask = employeeRepo.AnyAsync(e => e.NationalId == request.Dto.NationalId, cancellationToken);
                var emailTask = _userManager.FindByEmailAsync(request.Dto.Email);
                var usernameTask = _userManager.FindByNameAsync(request.Dto.UserName);
                var phoneTask = _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == request.Dto.PhoneNumber, cancellationToken);

                await Task.WhenAll(nationalIdTask, emailTask, usernameTask, phoneTask);

                var duplicateErrors = new List<string>();
                if (emailTask.Result != null) duplicateErrors.Add("Email is already in use.");
                if (usernameTask.Result != null) duplicateErrors.Add("Username is already in use.");
                if (phoneTask.Result) duplicateErrors.Add("Phone number is already in use.");
                if (nationalIdTask.Result) duplicateErrors.Add("Employee with this National ID already exists.");

                if (duplicateErrors.Count > 0)
                {
                    _logger.LogWarning("Duplicate data found: {Errors}", string.Join(", ", duplicateErrors));
                    throw new BadRequestException(duplicateErrors);
                }

                newUser = new ApplicationUser
                {
                    UserName = request.Dto.UserName,
                    Email = request.Dto.Email,
                    PhoneNumber = request.Dto.PhoneNumber,
                    Address = request.Dto.Address,
                    FullName = $"{request.Dto.FirstName} {request.Dto.LastName}",
                    Gender = request.Dto.Gender,
                    DateOfBirth = request.Dto.DateOfBirth,
                };

                var identityResult = await _userManager.CreateAsync(newUser, request.Dto.Password);
                if (!identityResult.Succeeded)
                {
                    _logger.LogWarning("Failed to create Identity user: {Errors}", string.Join(", ", identityResult.Errors.Select(e => e.Description)));
                    throw new BadRequestException(identityResult.Errors.Select(e => e.Description).ToList());
                }

                _logger.LogDebug("Identity user created successfully. UserId: {UserId}", newUser.Id);

                var employee = await HandleEmployeeUserAsync(request, newUser, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Employee created successfully. EmployeeId: {EmployeeId}, UserId: {UserId}, CreatedBy: {CreatedBy}",
                    employee.Id, newUser.Id, request.CreatedBy);

                return _mapper.Map<EmployeeDto>(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating employee.");

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                if (newUser != null)
                {
                    await _userManager.DeleteAsync(newUser);
                    _logger.LogInformation("Rolled back partially created Identity user. UserId: {UserId}", newUser.Id);
                }

                throw;
            }
        }

        private async Task<Employee> HandleEmployeeUserAsync(
            CreateEmployeeCommand request,
            ApplicationUser newUser,
            CancellationToken cancellationToken)
        {
            var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();

            var employee = new Employee
            {
                UserId = newUser.Id,
                FirstName = request.Dto.FirstName,
                LastName = request.Dto.LastName,
                NationalId = request.Dto.NationalId,
                JobTitle = request.Dto.JobTitle,
                BaseSalary = request.Dto.BaseSalary,
                HireDate = request.Dto.HireDate,
                DepartmentId = request.Dto.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy
            };

            await employeeRepo.AddAsync(employee, cancellationToken);

            var role = await _roleManager.FindByIdAsync(request.Dto.RoleId);
            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", request.Dto.RoleId);
                throw new NotFoundException<IdentityRole>(request.Dto.RoleId);
            }
            await _userManager.AddToRoleAsync(newUser, role.Name!);

            return employee;
        }
    }
}
