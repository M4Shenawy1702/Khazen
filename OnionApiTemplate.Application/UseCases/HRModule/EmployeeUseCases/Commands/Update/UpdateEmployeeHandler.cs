using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.EmployeeUsecases.Commands.Update
{
    internal class UpdateEmployeeHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateEmployeeCommand> validator,
        UserManager<ApplicationUser> userManager,
        ILogger<UpdateEmployeeHandler> logger
    ) : IRequestHandler<UpdateEmployeeCommand, EmployeeDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateEmployeeCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<UpdateEmployeeHandler> _logger = logger;

        public async Task<EmployeeDetailsDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting UpdateEmployeeHandler for EmployeeId: {EmployeeId}, ModifiedBy: {ModifiedBy}",
                request.Id, request.ModifiedBy);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var modifyingUser = await _userManager.FindByNameAsync(request.ModifiedBy);
            if (modifyingUser == null)
            {
                _logger.LogWarning("User not found: {ModifiedBy}", request.ModifiedBy);
                throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
                var employee = await employeeRepo.GetAsync(new GetEmployeeWithUserByIdSpecification(request.Id), cancellationToken)
                    ?? throw new NotFoundException<Employee>(request.Id);

                _logger.LogDebug("Fetched employee: {EmployeeId}, UserId: {UserId}", employee.Id, employee.UserId);

                var emailTask = _userManager.Users
                    .Where(u => u.Id != employee.UserId && u.Email == request.Dto.Email)
                    .AsNoTracking()
                    .AnyAsync(cancellationToken);

                var usernameTask = _userManager.Users
                    .Where(u => u.Id != employee.UserId && u.UserName == request.Dto.UserName)
                    .AsNoTracking()
                    .AnyAsync(cancellationToken);

                var phoneTask = _userManager.Users
                    .Where(u => u.Id != employee.UserId && u.PhoneNumber == request.Dto.PhoneNumber)
                    .AsNoTracking()
                    .AnyAsync(cancellationToken);

                var nationalIdTask = employeeRepo.AnyAsync(e => e.NationalId == request.Dto.NationalId && e.Id != employee.Id, cancellationToken);

                await Task.WhenAll(emailTask, usernameTask, phoneTask, nationalIdTask);

                var conflicts = new List<string>();
                if (emailTask.Result) conflicts.Add($"Email '{request.Dto.Email}' is already in use.");
                if (usernameTask.Result) conflicts.Add($"Username '{request.Dto.UserName}' is already in use.");
                if (phoneTask.Result) conflicts.Add($"Phone number '{request.Dto.PhoneNumber}' is already in use.");
                if (nationalIdTask.Result) conflicts.Add($"National ID '{request.Dto.NationalId}' is already in use.");

                if (conflicts.Count > 0)
                {
                    _logger.LogWarning("Duplicate data found for EmployeeId {EmployeeId}: {Errors}", employee.Id, string.Join(", ", conflicts));
                    throw new ConflictException(conflicts);
                }

                UpdateEmployee(request, employee);
                employeeRepo.Update(employee);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Employee updated successfully. EmployeeId: {EmployeeId}, ModifiedBy: {ModifiedBy}",
                    employee.Id, request.ModifiedBy);

                return _mapper.Map<EmployeeDetailsDto>(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee: {EmployeeId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private static void UpdateEmployee(UpdateEmployeeCommand request, Employee employee)
        {
            employee.User!.FullName = $"{request.Dto.FirstName} {request.Dto.LastName}";
            employee.User.DateOfBirth = request.Dto.DateOfBirth;
            employee.User.Address = request.Dto.Address;
            employee.User.Gender = request.Dto.Gender;
            employee.User.PhoneNumber = request.Dto.PhoneNumber;
            employee.User.Email = request.Dto.Email;
            employee.User.UserName = request.Dto.UserName;

            employee.FirstName = request.Dto.FirstName;
            employee.LastName = request.Dto.LastName;
            employee.NationalId = request.Dto.NationalId;
            employee.HireDate = request.Dto.HireDate;
            employee.JobTitle = request.Dto.JobTitle;
            employee.BaseSalary = request.Dto.BaseSalary;
            employee.ModifiedAt = DateTime.UtcNow;
            employee.ModifiedBy = request.ModifiedBy;
        }
    }
}
