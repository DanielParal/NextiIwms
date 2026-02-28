using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Depositors;

internal class DepositorConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<DepositorProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Depositor>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                })
            .Index(x => x.DeliveryTemplateCode)
            .Index(x => x.LoadingTemplateCode);
        
        options.Schema.For<Depositor>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Depositor>());
    }
}