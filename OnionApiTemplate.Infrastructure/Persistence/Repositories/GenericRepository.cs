using Khazen.Domain.Entities;
using Khazen.Domain.IRepositoty;
using System.Linq.Expressions;

namespace Khazen.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey>(ApplicationDbContext _context)
        : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        public async System.Threading.Tasks.Task AddAsync(TEntity entity, CancellationToken cancellationToken)
            => await _context.Set<TEntity>().AddAsync(entity, cancellationToken);

        public void Update(TEntity entity)
            => _context.Set<TEntity>().Update(entity);
        public void UpdateRange(IEnumerable<TEntity> entities)
                => _context.Set<TEntity>().RemoveRange(entities);
        public void Delete(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges)
            => trackChanges
                ? await _context.Set<TEntity>().ToListAsync(cancellationToken)
                : await _context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);

        public async Task<TEntity?> GetByIdAsync(TKey key, CancellationToken cancellationToken, bool trackChanges = false)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(e => e.Id!.Equals(key), cancellationToken);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken)
            => await _context.Set<TEntity>().SingleOrDefaultAsync(cancellationToken);

        public async Task<bool> AnyAsync(CancellationToken cancellationToken)
            => await _context.Set<TEntity>().AnyAsync(cancellationToken);

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken, bool asNoTracking = false)
            => await SpecificationEvaluator
                .GetQuery(_context.Set<TEntity>(), specification, asNoTracking)
                .ToListAsync(cancellationToken);

        public async Task<TEntity?> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken, bool asNoTracking = false)
            => await SpecificationEvaluator
                .GetQuery(_context.Set<TEntity>(), specification, asNoTracking)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<int> GetCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken, bool asNoTracking = true)
            => await SpecificationEvaluator
                .GetQuery(_context.Set<TEntity>(), specification, asNoTracking)
                .CountAsync(cancellationToken);

    }
}
