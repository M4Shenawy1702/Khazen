namespace Khazen.Application.Common.QueryParameters
{
    public class DepartmentQueryParameters
    {
        private const int MaxPageSize = 50;
        private const int DefaultPageSize = 10;

        public string? DepartmentName { get; set; }
        public Guid? DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;
        public int PageIndex { get; set; } = 1;

        private int _pageSize = DefaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 0 && value <= MaxPageSize ? value : DefaultPageSize;
        }
    }
}
