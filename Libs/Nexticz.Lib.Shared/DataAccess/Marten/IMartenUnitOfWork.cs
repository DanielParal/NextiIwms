
namespace Nexticz.Lib.Shared.DataAccess.Marten;

public interface IMartenUnitOfWork : IDisposable
{
    bool IsExplicitTransaction { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void BeginTransaction();
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
    Guid StartStream<TEvent, TEntity>(Guid streamId, TEvent @event) where TEntity : class;
    Guid AppendEvent<T>(Guid streamId, T @event) where T : class;
}