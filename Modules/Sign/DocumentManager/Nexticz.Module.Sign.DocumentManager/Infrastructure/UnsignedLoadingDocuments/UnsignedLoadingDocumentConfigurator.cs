using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.UnsignedLoadingDocuments;

internal class UnsignedLoadingDocumentConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<UnsignedLoadingDocumentProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<UnsignedLoadingDocumentView>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<UnsignedLoadingDocumentView>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<UnsignedLoadingDocumentView>());
    }
}