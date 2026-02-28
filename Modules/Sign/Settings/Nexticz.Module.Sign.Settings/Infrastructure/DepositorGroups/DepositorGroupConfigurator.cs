using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.DepositorGroups;

internal class DepositorGroupConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<DepositorGroupProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<DepositorGroup>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<DepositorGroup>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<DepositorGroup>());
    }
}