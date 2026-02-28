using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.Manufactures;

internal class ManufactureConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ManufactureProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Manufacture>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<Manufacture>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Manufacture>());
    }
}