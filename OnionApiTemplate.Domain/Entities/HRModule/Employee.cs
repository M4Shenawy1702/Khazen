using Khazen.Domain.Entities.ReportsModule;
using Khazen.Domain.Entities.UsersModule;

namespace Khazen.Domain.Entities.HRModule
{
    public class Employee : BaseEntity<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public DateOnly HireDate { get; set; }
        public JobTitle JobTitle { get; set; }
        public decimal BaseSalary { get; set; }

        public Guid DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = [];
        public ICollection<PerformanceReview> PerformanceReviews { get; set; } = [];
        public ICollection<SavedReport> SavedReports { get; set; } = [];
        public ICollection<Salary> Salaries { get; set; } = [];
        public ICollection<Bonus> Bonuses { get; set; } = [];
        public ICollection<Advance> Advances { get; set; } = [];
        public ICollection<Deduction> Deductions { get; set; } = [];

        public Bonus CreateBonus(decimal amount, string reason, DateOnly date, string createdBy)
        {
            var bonus = new Bonus
            {
                Amount = amount,
                Reason = reason,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = this.Id,
                Date = date,
                CreatedBy = createdBy,
            };

            Bonuses.Add(bonus);
            return bonus;
        }

        public Deduction CreateDeduction(decimal amount, string reason, DateOnly date, string createdBy)
        {
            var deduction = new Deduction
            {
                Amount = amount,
                Reason = reason,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = this.Id,
                Date = date,
                CreatedBy = createdBy
            };

            Deductions.Add(deduction);
            return deduction;
        }

        public Advance CreateAdvance(decimal amount, string reason, DateOnly date, string createdBy)
        {
            var advance = new Advance
            {
                Amount = amount,
                Reason = reason,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = this.Id,
                Date = date,
                CreatedBy = createdBy
            };

            Advances.Add(advance);
            return advance;
        }
    }

    public enum JobTitle
    {
        Manager,
        Employee,
        Intern,
        Trainee,
        Other
    }
}


