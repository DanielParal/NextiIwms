using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Infrastructure.EmailMessages;

internal class EmailMessageConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<EmailMessageProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<EmailMessage>();
        
        options.Schema.For<EmailMessage>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<EmailMessage>());
    }
}