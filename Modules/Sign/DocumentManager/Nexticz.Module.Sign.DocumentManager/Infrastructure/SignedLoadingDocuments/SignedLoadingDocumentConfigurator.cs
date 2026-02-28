using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.SignedLoadingDocuments;

internal class SignedLoadingDocumentConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<SignedLoadingDocumentProjection>(ProjectionLifecycle.Async);
        
        options.Schema.For<SignedLoadingDocumentView>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<SignedLoadingDocumentView>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<SignedLoadingDocumentView>());
    }
}