using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.PackagingCirculations;

internal class PackagingCirculationConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<PackagingCirculationProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<PackagingCirculation>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<PackagingCirculation>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<PackagingCirculation>());
    }
}