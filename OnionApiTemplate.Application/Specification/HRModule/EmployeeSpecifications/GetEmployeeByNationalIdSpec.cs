namespace Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications
{
    internal class GetEmployeeByNationalIdSpec
        : BaseSpecifications<Employee>
    {
        public GetEmployeeByNationalIdSpec(string nationalId)
            : base(e => e.NationalId == nationalId)
        {
        }
    }
}
