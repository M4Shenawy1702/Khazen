public record SalaryDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public DateOnly SalaryDate { get; init; }
    public decimal BasicSalary { get; init; }
    public decimal TotalBonus { get; set; }
    public decimal TotalDeduction { get; set; }
    public decimal TotalAdvance { get; set; }
    public decimal NetSalary { get; init; }
    public string Notes { get; init; } = string.Empty;

    public SalaryDto() { }
}
