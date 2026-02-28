using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.EmailSender.Domain.Views;

namespace Nexticz.Module.EmailSender.Infrastructure.ScheduledEmailMessages;

internal class ScheduledEmailMessageConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ScheduledEmailMessageProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<ScheduledEmailMessageView>()
            .Index(x => x.ScheduledToBeSentAt);
        
        options.Schema.For<ScheduledEmailMessageView>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<ScheduledEmailMessageView>());
    }
}