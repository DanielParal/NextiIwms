using Marten;

namespace Nexticz.Lib.Shared.DataAccess.Marten;

public interface IMartenDocumentSessionProvider : IDisposable
{
    IDocumentStore GetStore();
    IDocumentSession GetSession();
    IQuerySession GetQuerySession();
    void Rollback();
}