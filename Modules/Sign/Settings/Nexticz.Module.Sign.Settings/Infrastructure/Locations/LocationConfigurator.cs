using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Locations;

internal class LocationConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<LocationProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Location>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<Location>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Location>());
    }
}