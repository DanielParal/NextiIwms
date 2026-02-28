using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.SentEmails;

internal class SentEmailConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<SentEmailProjection>(ProjectionLifecycle.Async);
        
        options.Schema.For<SentEmailView>()
            .Index(x => x.Recipients);
        
        options.Schema.For<SentEmailView>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<SentEmailView>());
    }
}