using Marten;

namespace Nexticz.Lib.Shared.DataAccess.Marten;

public class MartenDocumentSessionProvider(IDocumentStore documentStore) : IMartenDocumentSessionProvider
{
    private IDocumentSession? _session;

    public IDocumentStore GetStore()
    {
        return documentStore;   
    }

    public IDocumentSession GetSession()
    {
        if (_session == null)
            _session = documentStore.IdentitySession();

        return _session;
    }

    public IQuerySession GetQuerySession()
    {
        return documentStore.QuerySession();
    }

    public void Rollback()
    {
        _session?.Dispose();
        _session = null;
    }
    
    public void Dispose()
    {
        _session?.Dispose();
        _session = null;
    }
}