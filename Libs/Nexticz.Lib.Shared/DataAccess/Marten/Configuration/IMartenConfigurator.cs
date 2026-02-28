using Marten;

namespace Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

public interface IMartenConfigurator
{
    void Configure(StoreOptions options);
}