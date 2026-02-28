using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.EmailTemplates;

internal class EmailTemplateConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<EmailTemplateProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<EmailTemplate>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<EmailTemplate>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<EmailTemplate>());
    }
}