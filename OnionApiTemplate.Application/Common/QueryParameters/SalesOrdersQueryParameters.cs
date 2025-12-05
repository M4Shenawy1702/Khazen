using Khazen.Domain.Common.Enums;

namespace Khazen.Application.Common.QueryParameters
{
    public class SalesOrdersQueryParameters
    {
        private const int maxPageSize = 50;
        private const int defaultPageSize = 10;

        //public DateOnly? StartDate { get; set; }
        //public DateOnly? EndDate { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }
        public int PageIndex { get; set; } = 1;
        private int _pageSize = defaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > maxPageSize ? maxPageSize : value;
        }
    }
}
