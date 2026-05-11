namespace Khazen.Application.Common
{
    public class PaginatedResult<T>
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public IEnumerable<T> Items { get; }

        public PaginatedResult(int pageIndex, int pageSize, int totalCount, IEnumerable<T> items)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            Items = items;
        }

        public static PaginatedResult<T> Empty(int pageIndex, int pageSize)
        {
            return new PaginatedResult<T>(
                pageIndex: pageIndex,
                pageSize: pageSize,
                totalCount: 0,
                items: Enumerable.Empty<T>()
            );
        }
    }
}