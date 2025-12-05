namespace Khazen.Application.BaseSpecifications.HRModule.DepartmentSpecifications
{
    internal class GetDepartmentByIdSpecification
        : BaseSpecifications<Department>
    {
        public GetDepartmentByIdSpecification(Guid id)
            : base(d => d.Id == id)
        {
            AddInclude(d => d.Employees);
        }
    }
}
