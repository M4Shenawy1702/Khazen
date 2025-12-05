using System.Linq.Expressions;

namespace Khazen.Domain.IRepositoty
{
    public interface ISpecification<T> where T : class
    {
        Expression<Func<T, bool>>? Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDesc { get; }
        int Skip { get; }
        int Take { get; }
        bool IsPaginated { get; }
    }
}
