namespace Khazen.Domain.Exceptions
{
    public class SalaryAlreadyExistsException(Guid employeeId, DateOnly salaryDate)
    : BadRequestException([$"Salary for employee with id: {employeeId} already exists for this month: {salaryDate}"]);
}
