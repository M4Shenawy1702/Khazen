namespace Khazen.Application.Common.QueryParameters
{
    public abstract class QueryParametersBaseClass
    {
        private const int maxPageSize = 50;
        private const int defaultPageSize = 10;

        public bool? IsDeleted { get; set; }
        public int PageIndex { get; set; } = 1;
        private int _pageSize = defaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > maxPageSize ? maxPageSize : value;
        }
    }
}
