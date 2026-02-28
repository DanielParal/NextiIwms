using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.KitSapDefinitions;

internal class KitSapDefinitionConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<KitSapDefinitionProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<KitSapDefinition>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<KitSapDefinition>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<KitSapDefinition>());
    }
}