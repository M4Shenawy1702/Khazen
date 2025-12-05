using Khazen.Domain.Entities;
using System.Linq.Expressions;

namespace Khazen.Domain.IRepositoty
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);

        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges);
        Task<TEntity?> GetByIdAsync(TKey key, CancellationToken cancellationToken, bool trackChanges = false);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken);
        Task<bool> AnyAsync(CancellationToken cancellationToken);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken, bool asNoTracking = false);
        Task<TEntity?> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken, bool asNoTracking = false);
        Task<int> GetCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken, bool asNoTracking = true);
    }
}
