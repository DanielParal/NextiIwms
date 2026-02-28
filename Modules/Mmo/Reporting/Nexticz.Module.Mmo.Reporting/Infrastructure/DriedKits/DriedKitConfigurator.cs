using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;


namespace Nexticz.Module.Mmo.Reporting.Infrastructure.DriedKits;

internal class DriedKitConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<DriedKitProjection>(ProjectionLifecycle.Async);
        options.Schema.For<DriedKit>().Index(x => x.KitIdFromDrying);
        options.Schema.For<DriedKit>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<DriedKit>());
    }
}