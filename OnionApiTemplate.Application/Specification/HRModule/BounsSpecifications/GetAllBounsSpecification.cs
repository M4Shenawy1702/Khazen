using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.BounsSpecifications
{
    internal class GetAllBounsSpecification : BaseSpecifications<Bonus>
    {
        public GetAllBounsSpecification(BonusQueryParameters queryParameters)
           : base(r =>
                (!queryParameters.EmployeeId.HasValue ||
                    r.EmployeeId == queryParameters.EmployeeId.Value) &&

                (!queryParameters.From.HasValue ||
                    r.Date >= queryParameters.From.Value) &&

                (!queryParameters.To.HasValue ||
                    r.Date <= queryParameters.To.Value)
            )
        {
            AddInclude(r => r.Employee!);
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
