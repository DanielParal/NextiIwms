using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Weasel.Postgresql.Tables;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Packagings;

internal class PackagingConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<PackagingProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Packaging>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
                
        options.Schema.For<Packaging>().Index(x => x.WashingMachineSpeeds, 
            idx =>
            {
                idx.Method = IndexMethod.gin;
            });
        
        options.Schema.For<Packaging>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Packaging>());
    }
}