using Khazen.Domain.IRepositoty;

namespace Khazen.Infrastructure.Persistence.Repositories
{
    internal class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> spec, bool asNoTracking = false) where T : class
        {
            var query = inputQuery;

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            if (spec.Includes != null && spec.Includes.Count != 0)
                query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            // Apply string-based includes (ThenInclude support)
            if (spec.IncludeStrings != null && spec.IncludeStrings.Count != 0)
                query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDesc != null)
                query = query.OrderByDescending(spec.OrderByDesc);

            if (spec.IsPaginated)
                query = query.Skip(spec.Skip).Take(spec.Take);

            if (asNoTracking)
                query = query.AsNoTracking();

            return query;
        }
    }
}
