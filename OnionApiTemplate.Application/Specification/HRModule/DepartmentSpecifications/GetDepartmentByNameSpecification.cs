namespace Khazen.Application.BaseSpecifications.HRModule.DepartmentSpecifications
{
    internal class GetDepartmentByNameSpecification : BaseSpecifications<Department>
    {
        public GetDepartmentByNameSpecification(string name)
            : base(d => d.Name == name)
        {
            AddInclude(d => d.Employees);
        }
    }
}
