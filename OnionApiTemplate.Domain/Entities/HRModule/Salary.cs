using Khazen.Domain.Exceptions;

namespace Khazen.Domain.Entities.HRModule
{
    public class Salary : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateTime SalaryDate { get; set; }

        public decimal BasicSalary { get; private set; }
        public decimal TotalBonus { get; private set; }
        public decimal TotalDeduction { get; private set; }
        public decimal TotalAdvance { get; private set; }
        public decimal NetSalary { get; private set; }

        public string Notes { get; private set; } = string.Empty;

        private Salary() { }

        public static Salary Create(
            Guid employeeId,
            DateTime salaryDate,
            decimal basicSalary,
            string createdBy,
            decimal bonus = 0,
            decimal deduction = 0,
            decimal advance = 0,
            string? notes = null)
        {
            ValidateAmounts(basicSalary, bonus, deduction, advance);

            var salary = new Salary
            {
                EmployeeId = employeeId,
                SalaryDate = salaryDate,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            salary.UpdateCompensation(basicSalary, bonus, deduction, advance, notes);
            return salary;
        }

        public void UpdateCompensation(
            decimal basicSalary,
            decimal bonus,
            decimal deduction,
            decimal advance,
            string? notes = null)
        {
            ValidateAmounts(basicSalary, bonus, deduction, advance);

            BasicSalary = basicSalary;
            TotalBonus = bonus;
            TotalDeduction = deduction;
            TotalAdvance = advance;

            if (!string.IsNullOrWhiteSpace(notes))
                Notes = notes!;

            NetSalary = (BasicSalary + TotalBonus) - (TotalDeduction + TotalAdvance);
            if (NetSalary < 0)
                NetSalary = 0;
        }

        public void UpdateNotes(string? notes)
        {
            if (!string.IsNullOrWhiteSpace(notes))
                Notes = notes!;
        }

        private static void ValidateAmounts(decimal basicSalary, decimal bonus, decimal deduction, decimal advance)
        {
            if (basicSalary < 0 || bonus < 0 || deduction < 0 || advance < 0)
                throw new DomainException("Salary components cannot be negative.");
        }

        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}
