using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.Common.Interfaces.IHRModule.IEmployeeServices;
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
        ILogger<UpdateEmployeeHandler> logger,
        IEmployeeDomainServices employeeDomainServices
    ) : IRequestHandler<UpdateEmployeeCommand, EmployeeDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateEmployeeCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<UpdateEmployeeHandler> _logger = logger;
        private readonly IEmployeeDomainServices _employeeDomainServices = employeeDomainServices;

        public async Task<EmployeeDetailsDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting UpdateEmployeeHandler for EmployeeId: {EmployeeId}, ModifiedBy: {ModifiedBy}",
                request.Id, request.CurrentUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {ModifiedBy}", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
                var employee = await employeeRepo.GetAsync(new GetEmployeeWithUserByIdSpecification(request.Id), cancellationToken);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found: {EmployeeId}", request.Id);
                    throw new NotFoundException<Employee>(request.Id);
                }

                _logger.LogDebug("Fetched employee: {EmployeeId}, UserId: {UserId}", employee.Id, employee.UserId);

                await CheckForIdentityConflictsAsync(request, employee, cancellationToken);

                await UpdateIdentityUserAsync(request, employee, cancellationToken);

                _employeeDomainServices.UpdateEmployeeFields(request, employee);

                employeeRepo.Update(employee);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Employee updated successfully. EmployeeId: {EmployeeId}, ModifiedBy: {ModifiedBy}",
                    employee.Id, request.CurrentUserId);

                return _mapper.Map<EmployeeDetailsDto>(employee);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database constraint exception during update for EmployeeId: {EmployeeId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new ConflictException("A database constraint was violated. Likely duplicate identity fields.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating employee: {EmployeeId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private async Task CheckForIdentityConflictsAsync(UpdateEmployeeCommand request, Employee employee, CancellationToken ct)
        {
            var emailTask = _userManager.Users
                .Where(u => u.Id != employee.UserId && u.Email == request.Dto.Email)
                .AsNoTracking()
                .AnyAsync(ct);

            var usernameTask = _userManager.Users
                .Where(u => u.Id != employee.UserId && u.UserName == request.Dto.UserName)
                .AsNoTracking()
                .AnyAsync(ct);

            var phoneTask = _userManager.Users
                .Where(u => u.Id != employee.UserId && u.PhoneNumber == request.Dto.PhoneNumber)
                .AsNoTracking()
                .AnyAsync(ct);

            var nationalIdTask = _unitOfWork.GetRepository<Employee, Guid>()
                .AnyAsync(e => e.NationalId == request.Dto.NationalId && e.Id != employee.Id, ct);

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
        }

        private async Task UpdateIdentityUserAsync(UpdateEmployeeCommand request, Employee employee, CancellationToken ct)
        {
            var identityUser = employee.User!;

            identityUser.FullName = $"{request.Dto.FirstName} {request.Dto.LastName}".Trim().Replace("  ", " ");
            identityUser.Email = request.Dto.Email;
            identityUser.UserName = request.Dto.UserName;
            identityUser.PhoneNumber = request.Dto.PhoneNumber;
            identityUser.Address = request.Dto.Address;
            identityUser.DateOfBirth = request.Dto.DateOfBirth;
            identityUser.Gender = request.Dto.Gender;

            var identityResult = await _userManager.UpdateAsync(identityUser);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToList();
                throw new ConflictException(errors);
            }
        }


    }
}
