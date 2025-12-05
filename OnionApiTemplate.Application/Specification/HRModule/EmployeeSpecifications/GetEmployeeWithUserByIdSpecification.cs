namespace Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications
{
    internal class GetEmployeeWithUserByIdSpecification
        : BaseSpecifications<Employee>
    {
        public GetEmployeeWithUserByIdSpecification(Guid employeeId)
            : base(e => e.Id == employeeId)
        {
            AddInclude(e => e.User!);
        }
    }
}
