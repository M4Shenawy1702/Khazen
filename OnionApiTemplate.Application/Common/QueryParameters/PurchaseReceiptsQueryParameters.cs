namespace Khazen.Application.Common.QueryParameters
{
    public class PurchaseReceiptsQueryParameters : QueryParametersBaseClass
    {
        public DateTime? StatDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
