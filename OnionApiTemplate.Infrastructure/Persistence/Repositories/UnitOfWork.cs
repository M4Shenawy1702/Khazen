using Khazen.Domain.Entities;
using Khazen.Domain.IRepositoty;
using Microsoft.EntityFrameworkCore.Storage;

namespace Khazen.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork(ApplicationDbContext context)
        : IUnitOfWork, IAsyncDisposable
    {
        private readonly ApplicationDbContext _context = context;
        private IDbContextTransaction? _transaction;

        private readonly Dictionary<Type, object> _repositories = [];
        public DbContext Context => _context;
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
        {
            var entityType = typeof(TEntity);

            if (!_repositories.TryGetValue(entityType, out var repository))
            {
                repository = new GenericRepository<TEntity, TKey>(_context);
                _repositories[entityType] = repository;
            }

            return (IGenericRepository<TEntity, TKey>)repository!;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null) return;
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await (_transaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await (_transaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask);
            }
            finally
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
                     await _context.SaveChangesAsync(cancellationToken);

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }
    }
}
