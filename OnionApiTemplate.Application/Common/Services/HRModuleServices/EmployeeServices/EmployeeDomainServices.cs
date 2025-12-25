using Khazen.Application.Common.Interfaces.IHRModule.IEmployeeServices;
using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Commands.Update;
using Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.Create;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.HRModuleServices.EmployeeServices
{
    internal class EmployeeDomainServices(IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<EmployeeDomainServices> logger)
        : IEmployeeDomainServices
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<EmployeeDomainServices> _logger = logger;

        public async Task<Employee> CreateEmployeeAsync(CreateEmployeeCommand request, ApplicationUser newUser, CancellationToken ct)
        {
            if (request.Dto.BaseSalary < 0)
            {
                _logger.LogError("Domain Service: Validation failed. Base salary cannot be negative for User {UserId}", newUser.Id);
                throw new BadRequestException("Base salary must be a positive value.");
            }

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
                CreatedBy = request.CurrentUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Employee, Guid>().AddAsync(employee, ct);

            _logger.LogInformation("Domain Service: Employee entity created for UserId: {UserId}", newUser.Id);
            return employee;
        }
        public void UpdateEmployeeFields(UpdateEmployeeCommand request, Employee employee)
        {
            employee.FirstName = request.Dto.FirstName;
            employee.LastName = request.Dto.LastName;
            employee.NationalId = request.Dto.NationalId;
            employee.HireDate = request.Dto.HireDate;
            employee.JobTitle = request.Dto.JobTitle;
            employee.BaseSalary = request.Dto.BaseSalary;
            employee.ModifiedAt = DateTime.UtcNow;
            employee.ModifiedBy = request.CurrentUserId;
        }
    }
}
