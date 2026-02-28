using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.InactivityTypes;

internal class InactivityTypeConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<InactivityTypeProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<InactivityType>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<InactivityType>());
    }
}