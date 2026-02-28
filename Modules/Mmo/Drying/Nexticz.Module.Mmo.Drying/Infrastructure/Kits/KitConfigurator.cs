using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Infrastructure.Kits;

internal class KitConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<KitProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Kit>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Kit>());
    }
}