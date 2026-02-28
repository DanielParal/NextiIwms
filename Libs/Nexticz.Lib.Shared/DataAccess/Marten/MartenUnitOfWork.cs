using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Lib.Shared.Logging;
using Nexticz.Lib.Shared.UserProviders;

namespace Nexticz.Lib.Shared.DataAccess.Marten;

public abstract class MartenUnitOfWork(
    IMartenDocumentSessionProvider documentSessionProvider,
    ICurrentUserProvider currentUserProvider)
    : IMartenUnitOfWork
{
    private IDocumentSession DocumentSession => documentSessionProvider.GetSession();
    public bool IsExplicitTransaction { get; private set; }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (IsExplicitTransaction)
            throw new InvalidOperationException(
                "MartenUnitOfWork - Cannot call SaveChangesAsync directly when in an explicit transaction. " +
                "Use CommitTransactionAsync instead.");
        
        await CommitTransactionAsync(cancellationToken);
    }

    public void BeginTransaction() => IsExplicitTransaction = true;
    
    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        await DocumentSession.SaveChangesAsync(cancellationToken);
        IsExplicitTransaction = false;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        documentSessionProvider.Rollback();
        IsExplicitTransaction = false;
        return Task.CompletedTask;
    }
    
    public Guid StartStream<TEvent, TEntity>(Guid streamId, TEvent @event) where TEntity : class
    {
        SetHeaders();
        DocumentSession.Events.StartStream<TEntity>(streamId, @event);
        return streamId;
    }

    public Guid AppendEvent<T>(Guid streamId, T @event) where T : class
    {
        SetHeaders();
        DocumentSession.Events.Append(streamId, @event);
        return streamId;
    }

    private void SetHeaders()
    {
        DocumentSession.SetHeader(MartenEventHeaderName.UserName, currentUserProvider.GetCurrentUser().UserName);
        DocumentSession.SetHeader(MartenEventHeaderName.CorrelationId, CorrelationIdProvider.Instance.GetInternalId());
    }

    public void Dispose()
    {
        documentSessionProvider.Dispose();
    }
    
    
}