namespace Khazen.Application.Common.QueryParameters
{
    public class EmployeeQueryParameters
    {
        private const int MaxPageSize = 50;
        private const int DefaultPageSize = 10;

        public string? Name { get; set; }

        public EmployeeSortOptions? SortOption { get; set; }
        public int PageIndex { get; set; } = 1;

        private int _pageSize = DefaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 0 && value <= MaxPageSize ? value : DefaultPageSize;
        }
    }
    public enum EmployeeSortOptions
    {
        EmployeeNameAscending,
        EmployeeNameDescending,
    }
}
