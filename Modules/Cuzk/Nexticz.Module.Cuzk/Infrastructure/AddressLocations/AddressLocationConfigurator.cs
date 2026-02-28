using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Infrastructure.AddressLocations;

internal class AddressLocationConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<AddressLocationProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<AddressLocation>().UniqueIndex(x => x.AdmCode);
        options.Schema.For<AddressLocation>().Index(x => x.MunicipalityCode);
        
        options.Schema.For<AddressLocation>()
            .DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<AddressLocation>());
    }
}