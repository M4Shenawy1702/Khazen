using Khazen.Application.Common.Interfaces.IHRModule.ISalaryServices;
using Khazen.Domain.Exceptions;
using static Khazen.Domain.Entities.HRModule.AttendanceRecord;

namespace Khazen.Application.Common.Services.HRModuleServices.SalaryServices
{
    public class SalaryDomainService : ISalaryDomainService
    {
        private const decimal HalfDayFactor = 0.5m;

        public Salary CreateSalary(Employee employee, DateTime salaryDate, string CurrentUserName, string? notes = null)
        {
            if (employee == null)
                throw new DomainException("Employee cannot be null when creating salary.");

            var daysInMonth = DateTime.DaysInMonth(salaryDate.Year, salaryDate.Month);
            if (daysInMonth <= 0)
                throw new DomainException("Invalid month or year for salary date.");

            var dailyRate = employee.BaseSalary / daysInMonth;

            var currentMonthAttendance = employee.AttendanceRecords
                .Where(a => a.Date.Month == salaryDate.Month && a.Date.Year == salaryDate.Year)
                .ToList() ?? new List<AttendanceRecord>();

            var absents = currentMonthAttendance.Count(a => a.Status == AttendanceStatus.Absent);
            var leaves = currentMonthAttendance.Count(a => a.Status == AttendanceStatus.Leave);

            var bonuses = employee.Bonuses?
                .Where(b => b.Date.Month == salaryDate.Month && b.Date.Year == salaryDate.Year)
                .Sum(b => b.Amount) ?? 0m;

            var deductions = employee.Deductions?
                .Where(d => d.Date.Month == salaryDate.Month && d.Date.Year == salaryDate.Year)
                .Sum(d => d.Amount) ?? 0m;

            var advances = employee.Advances?
                .Where(a => a.Date.Month == salaryDate.Month && a.Date.Year == salaryDate.Year)
                .Sum(a => a.Amount) ?? 0m;

            var totalAttendanceDeductions = (dailyRate * absents) + (dailyRate * HalfDayFactor * leaves);

            return Salary.Create(
                employee.Id,
                salaryDate,
                employee.BaseSalary,
                CurrentUserName,
                bonuses,
                deductions + totalAttendanceDeductions,
                advances,
                notes ?? string.Empty
            );
        }
    }
}
