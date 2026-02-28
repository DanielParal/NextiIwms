using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.KitTypes;

internal class KitTypeConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<KitTypeProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<KitType>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<KitType>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<KitType>());
    }
}