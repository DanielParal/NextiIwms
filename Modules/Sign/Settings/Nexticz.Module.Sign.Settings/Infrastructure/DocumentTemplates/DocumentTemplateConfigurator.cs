using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.DocumentTemplates;

internal class DocumentTemplateConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<DocumentTemplateProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<DocumentTemplate>().UniqueIndex(x => x.Code);
        options.Schema.For<DocumentTemplate>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<DocumentTemplate>());
    }
}