using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;
using Weasel.Postgresql.Tables;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Kits;

internal class KitConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<KitProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Kit>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
                
        options.Schema.For<Kit>().Index(x => x.PackagingCodeQuantities, 
            idx =>
            {
                idx.Method = IndexMethod.gin;
            });
        
        options.Schema.For<Kit>().Index(x => x.SpecialInformationSchedules);
        
        options.Schema.For<Kit>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Kit>());
    }
}