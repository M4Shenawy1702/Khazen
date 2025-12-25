using Khazen.Application.Common.Interfaces.IHRModule.IEmployeeServices;
using Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.Create;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.CreateEmployee
{
    internal class CreateEmployeeHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IValidator<CreateEmployeeCommand> validator,
        ILogger<CreateEmployeeHandler> logger,
        IEmployeeDomainServices employeeDomainServices
    ) : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IValidator<CreateEmployeeCommand> _validator = validator;
        private readonly ILogger<CreateEmployeeHandler> _logger = logger;
        private readonly IEmployeeDomainServices _employeeDomainServices = employeeDomainServices;

        public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating employee: {Email} in Dept: {DeptId}", request.Dto.Email, request.Dto.DepartmentId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogError("User {UserId} not found", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            ApplicationUser? newUser = null;
            try
            {
                var department = await _unitOfWork.GetRepository<Department, Guid>().GetByIdAsync(request.Dto.DepartmentId, cancellationToken);
                if (department == null) throw new NotFoundException<Department>(request.Dto.DepartmentId);
                if (department.IsDeleted) throw new BadRequestException("Cannot hire into an inactive department.");

                await CheckForDuplicatesAsync(request, cancellationToken);

                newUser = new ApplicationUser
                {
                    UserName = request.Dto.UserName,
                    Email = request.Dto.Email,
                    PhoneNumber = request.Dto.PhoneNumber,
                    FullName = $"{request.Dto.FirstName} {request.Dto.LastName}".Trim().Replace("  ", " "),
                    UserType = UserType.Employee,
                    IsActive = true
                };

                var identityResult = await _userManager.CreateAsync(newUser, request.Dto.Password);
                if (!identityResult.Succeeded) throw new BadRequestException(identityResult.Errors.Select(e => e.Description).ToList());

                var role = await _roleManager.FindByIdAsync(request.Dto.RoleId) ?? throw new NotFoundException<ApplicationRole>(request.Dto.RoleId);
                await _userManager.AddToRoleAsync(newUser, role.Name!);

                var employee = await _employeeDomainServices.CreateEmployeeAsync(request, newUser, cancellationToken);

                _logger.LogInformation("Successfully created employee: {Email}", request.Dto.Email);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return _mapper.Map<EmployeeDto>(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rollback initiated for employee creation: {Email}", request.Dto.Email);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                if (newUser != null && !string.IsNullOrEmpty(newUser.Id.ToString()))
                {
                    await _userManager.DeleteAsync(newUser);
                }
                throw;
            }
        }

        private async Task CheckForDuplicatesAsync(CreateEmployeeCommand request, CancellationToken ct)
        {
            var nationalIdTask = _unitOfWork.GetRepository<Employee, Guid>().AnyAsync(e => e.NationalId == request.Dto.NationalId, ct);
            var emailTask = _userManager.FindByEmailAsync(request.Dto.Email);
            var usernameTask = _userManager.FindByNameAsync(request.Dto.UserName);
            var phoneTask = _userManager.Users.AnyAsync(u => u.PhoneNumber == request.Dto.PhoneNumber, ct);

            await Task.WhenAll(nationalIdTask, emailTask, usernameTask, phoneTask);

            var duplicates = new List<string>();
            if (emailTask.Result != null) duplicates.Add("Email is already in use.");
            if (usernameTask.Result != null) duplicates.Add("Username is already in use.");
            if (phoneTask.Result) duplicates.Add("Phone number is already in use.");
            if (nationalIdTask.Result) duplicates.Add("National ID already exists.");

            if (duplicates.Count != 0) throw new BadRequestException(duplicates);
        }
    }
}