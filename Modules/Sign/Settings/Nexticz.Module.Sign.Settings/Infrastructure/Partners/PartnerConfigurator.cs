using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Partners;

internal class PartnerConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<PartnerProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Partner>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<Partner>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Partner>());
    }
}