using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSoses;

internal class WashingMachineSosConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<WashingMachineSosProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<WashingMachineSos>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<WashingMachineSos>());
    }
}