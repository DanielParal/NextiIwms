using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.Workers;

internal class WorkerConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<WorkerProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Worker>().Index(x => x.Pin, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<Worker>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Worker>());
    }
}