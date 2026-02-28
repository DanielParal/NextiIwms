using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.EmailConfigurations;

internal class EmailConfigurationConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<EmailConfigurationProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<EmailConfiguration>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<EmailConfiguration>());
    }
}