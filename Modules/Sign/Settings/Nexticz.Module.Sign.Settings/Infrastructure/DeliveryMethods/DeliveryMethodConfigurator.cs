using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.DeliveryMethods;

internal class DeliveryMethodConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<DeliveryMethodProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<DeliveryMethod>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<DeliveryMethod>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<DeliveryMethod>());
    }
}