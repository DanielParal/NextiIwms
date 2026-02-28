using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.Depositors;

internal class DepositorConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<DepositorProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Depositor>()
            .Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<Depositor>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Depositor>());
    }
}