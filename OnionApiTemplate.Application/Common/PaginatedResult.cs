namespace Khazen.Application.Common
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(int pageIndex, int pageSize, int totalCount, IEnumerable<T> items)
        {
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