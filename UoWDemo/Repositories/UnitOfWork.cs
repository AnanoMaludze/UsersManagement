using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using UsersManagement.Persistence;

namespace UsersManagement.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMainDbContext _databaseContext;
        private bool _disposed;
        private IDbContextTransaction _transaction;

        public UnitOfWork(IMainDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _transaction = await _databaseContext.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            await _databaseContext.SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            await _transaction?.RollbackAsync(cancellationToken);
        }

        public IRepository Repository()
        {
            return new Repository(_databaseContext);
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _databaseContext?.Dispose();
            _disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                Dispose();
            }
        }
    }
}