using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.LastEnteredWorkerOnLines;

internal class LastEnteredWorkerOnLineConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<LastEnteredWorkerOnLineProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<LastEnteredWorkerOnLine>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<LastEnteredWorkerOnLine>());
    }
}