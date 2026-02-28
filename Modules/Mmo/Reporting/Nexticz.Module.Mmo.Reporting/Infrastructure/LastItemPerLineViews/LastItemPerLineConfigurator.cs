using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Domain.Views;


namespace Nexticz.Module.Mmo.Reporting.Infrastructure.LastItemPerLineViews;

internal class LastItemPerLineConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<LastItemPerLineProjection>(ProjectionLifecycle.Async);
        
        options.Schema.For<LastItemPerLineView>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<LastItemPerLineView>());
    }
}