using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.LoadingDocuments;

internal class LoadingDocumentConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<LoadingDocumentProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<LoadingDocument>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<LoadingDocument>()
            .Index(x => x.SigningDeviceCode);
        
        options.Schema.For<LoadingDocument>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<LoadingDocument>());
    }
}