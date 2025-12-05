using Khazen.Application.Common.Interfaces.IHRModule;
using static Khazen.Domain.Entities.HRModule.AttendanceRecord;

namespace Khazen.Application.Common.Services
{
    public class SalaryCalculationService : ISalaryCalculationService
    {
        private const decimal HalfDayFactor = 0.5m;

        public decimal CalculateTotalDeductions(Employee employee, DateTime salaryDate)
        {
            var daysInMonth = DateTime.DaysInMonth(salaryDate.Year, salaryDate.Month);
            var dailyRate = employee.BaseSalary / daysInMonth;

            var currentMonthAttendance = employee.AttendanceRecords?
                .Where(a => a.Date.Month == salaryDate.Month && a.Date.Year == salaryDate.Year)
                .ToList() ?? new List<AttendanceRecord>();

            var absents = currentMonthAttendance.Count(a => a.Status == AttendanceStatus.Absent);
            var leaves = currentMonthAttendance.Count(a => a.Status == AttendanceStatus.Leave);

            var attendanceDeductions = (dailyRate * absents) + (dailyRate * HalfDayFactor * leaves);

            var monthlyDeductions = employee.Deductions?.Where(d => d.Date.Month == salaryDate.Month &&
                                                                    d.Date.Year == salaryDate.Year)
                                                        .Sum(d => d.Amount) ?? 0m;

            return monthlyDeductions + attendanceDeductions;
        }

        public decimal CalculateTotalBonuses(Employee employee, DateTime salaryDate)
        {
            return employee.Bonuses?.Where(b => b.Date.Month == salaryDate.Month && b.Date.Year == salaryDate.Year)
                                    .Sum(b => b.Amount) ?? 0m;
        }

        public decimal CalculateTotalAdvances(Employee employee, DateTime salaryDate)
        {
            return employee.Advances?.Where(a => a.Date.Month == salaryDate.Month && a.Date.Year == salaryDate.Year)
                                     .Sum(a => a.Amount) ?? 0m;
        }

        public decimal CalculateNetSalary(Employee employee, DateTime salaryDate)
        {
            var totalBonus = CalculateTotalBonuses(employee, salaryDate);
            var totalDeduction = CalculateTotalDeductions(employee, salaryDate);
            var totalAdvance = CalculateTotalAdvances(employee, salaryDate);

            return employee.BaseSalary + totalBonus - totalDeduction - totalAdvance;
        }
    }
}
