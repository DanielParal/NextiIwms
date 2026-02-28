using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.Constants;

internal class ConstantConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ConstantProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<Constant>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Constant>());
    }
}