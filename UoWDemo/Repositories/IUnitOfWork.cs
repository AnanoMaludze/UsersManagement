using Microsoft.Data.SqlClient;

namespace UsersManagement.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository Repository();
        Task<int> CommitAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);

    }
}
