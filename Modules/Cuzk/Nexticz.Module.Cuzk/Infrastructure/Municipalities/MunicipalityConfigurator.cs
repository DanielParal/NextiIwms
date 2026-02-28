using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Infrastructure.Municipalities;

internal class MunicipalityConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<MunicipalityProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<Municipality>().Index(x => x.Code);
        options.Schema.For<Municipality>().Index(x => x.ShouldImportAddressLocation);
        options.Schema.For<Municipality>()
            .DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Municipality>());
    }
}