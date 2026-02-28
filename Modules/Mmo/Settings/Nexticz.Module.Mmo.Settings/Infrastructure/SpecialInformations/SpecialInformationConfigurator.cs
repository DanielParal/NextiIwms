using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.SpecialInformations;

internal class SpecialInformationConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<SpecialInformationProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<SpecialInformation>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<SpecialInformation>());
    }
}