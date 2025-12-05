using Khazen.Application.DOTs.HRModule.AdvanceDtos;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Application.DOTs.HRModule.BonusDtos;
using Khazen.Application.DOTs.HRModule.Deduction;
using Khazen.Domain.Entities.ReportsModule;

namespace Khazen.Application.DOTs.HRModule.Employee
{
    public class EmployeeDetailsDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime LastLoginAt { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public DateOnly HireDate { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public decimal TotalBonuses { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalAdvances { get; set; }
        public decimal NetSalary { get; set; }

        public ICollection<AttendanceDto> AttendanceRecords { get; set; } = [];
        public ICollection<PerformanceReviewDto> PerformanceReviews { get; set; } = [];
        public ICollection<SavedReport> SavedReports { get; set; } = [];
        public ICollection<SalaryDto> Salaries { get; set; } = [];
        public ICollection<BonusDto> Bonuses { get; set; } = [];
        public ICollection<AdvanceDto> Advances { get; set; } = [];
        public ICollection<DeductionDto> Deductions { get; set; } = [];
    }
}
