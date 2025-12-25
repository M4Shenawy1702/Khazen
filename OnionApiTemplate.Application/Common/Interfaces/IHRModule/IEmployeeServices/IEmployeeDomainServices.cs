using Khazen.Application.UseCases.HRModule.EmployeeUsecases.Commands.Update;
using Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.Create;

namespace Khazen.Application.Common.Interfaces.IHRModule.IEmployeeServices
{
    internal interface IEmployeeDomainServices
    {
        public Task<Employee> CreateEmployeeAsync(CreateEmployeeCommand request, ApplicationUser newUser, CancellationToken ct);
        void UpdateEmployeeFields(UpdateEmployeeCommand request, Employee employee);
    }
}
