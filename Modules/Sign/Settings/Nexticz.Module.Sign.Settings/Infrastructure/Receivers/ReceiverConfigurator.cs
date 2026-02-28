using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Receivers;

internal class ReceiverConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ReceiverProjection>(ProjectionLifecycle.Inline);

        options.Schema.For<Receiver>()
            .Index(x => x.Code); 
                
        options.Schema.For<Receiver>()
            .Index(x => new { x.Code, x.PartnerCode }, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<Receiver>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Receiver>());
    }
}