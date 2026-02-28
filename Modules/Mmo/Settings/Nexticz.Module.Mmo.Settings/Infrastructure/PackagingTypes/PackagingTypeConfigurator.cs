using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.PackagingTypes;

internal class PackagingTypeConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<PackagingTypeProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<PackagingType>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<PackagingType>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<PackagingType>());
    }
}