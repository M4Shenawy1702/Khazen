namespace Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications
{
    internal class GetEmployeeByIdSpecification
        : BaseSpecifications<Employee>
    {
        public GetEmployeeByIdSpecification(Guid employeeId)
            : base(e => e.Id == employeeId)
        {
            AddInclude(e => e.Department!);
            AddInclude(e => e.User!);
            AddInclude(e => e.SavedReports);
            AddInclude(e => e.AttendanceRecords!);
            AddInclude(e => e.PerformanceReviews!);
            AddInclude(e => e.Salaries!);
            AddInclude(e => e.Bonuses);
            AddInclude(e => e.Deductions);
            AddInclude(e => e.Advances);
        }
    }
}
