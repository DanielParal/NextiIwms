using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.BaseRepositories;

internal class DocumentManagerUnitOfWork(
    IDocumentManagerDocumentSessionProvider documentSessionProvider,
    ICurrentUserProvider currentUserProvider)
    : Module.Sign.SharedKernel.DataAccess.UnitOfWork(documentSessionProvider, currentUserProvider), IDocumentManagerUnitOfWork
{
    public async Task<bool> RebuildProjectionAsync(string projectionTypeString, CancellationToken cancellationToken)
    {
        return await MartenRebuilder.RebuildProjectionAsync(
            documentSessionProvider.GetStore(), typeof(IDocumentManagerDocumentStore).Assembly, projectionTypeString, cancellationToken);
    }
}