using Khazen.Domain.Common.Enums;

namespace Khazen.Application.Common.QueryParameters
{
    public class CustomerQueryParameters : QueryParametersBaseClass
    {

        public CustomerType? CustomerType { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
