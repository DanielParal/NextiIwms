using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;


namespace Nexticz.Module.Mmo.Planning.Infrastructure.WashingMachines;

internal class WashingMachineConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<WashingMachineProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<WashingMachine>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<WashingMachine>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<WashingMachine>());
    }
}