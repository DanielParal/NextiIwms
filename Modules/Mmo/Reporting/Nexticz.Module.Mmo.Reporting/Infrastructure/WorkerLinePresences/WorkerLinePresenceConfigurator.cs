using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate;


namespace Nexticz.Module.Mmo.Reporting.Infrastructure.WorkerLinePresences;

internal class WorkerLinePresenceConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<WorkerLinePresenceProjection>(ProjectionLifecycle.Async);
        
        options.Schema.For<WorkerLinePresence>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<WorkerLinePresence>());
    }
}