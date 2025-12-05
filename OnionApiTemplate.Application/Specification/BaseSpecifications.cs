using System.Linq.Expressions;

namespace Khazen.Application.BaseSpecifications
{
    public abstract class BaseSpecifications<T> : ISpecification<T> where T : class
    {
        public Expression<Func<T, bool>>? Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; } = [];
        public List<string> IncludeStrings { get; } = [];
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDesc { get; private set; }

        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPaginated { get; private set; }

        protected BaseSpecifications(Expression<Func<T, bool>>? criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<T, object>> expression) => Includes.Add(expression);
        protected void AddInclude(string includeString) => IncludeStrings.Add(includeString);
        protected void AddOrderBy(Expression<Func<T, object>> expression) => OrderBy = expression;
        protected void AddOrderByDesc(Expression<Func<T, object>> expression) => OrderByDesc = expression;

        protected void ApplyPagination(int pageSize, int pageIndex)
        {
            IsPaginated = true;
            Take = pageSize;
            Skip = (pageIndex - 1) * pageSize;
        }
    }

}
