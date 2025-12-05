using Khazen.Domain.Common.Enums;

namespace Khazen.Application.Common.QueryParameters
{
    public class SalesInvoicesQueryParameters
    {
        private const int MaxPageSize = 50;
        private const int DefaultPageSize = 10;

        public Guid? CustomerId { get; set; }
        public InvoiceStatus? InvoiceStatus { get; set; }
        public int PageIndex { get; set; } = 1;

        private int _pageSize = DefaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 0 && value <= MaxPageSize ? value : DefaultPageSize;
        }
    }
}
