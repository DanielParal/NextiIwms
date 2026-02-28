using Nexticz.Module.Sign.DocumentLoader.Application.Interfaces;

namespace Nexticz.Module.Sign.DocumentLoader.Infrastructure.BaseRepositories;

internal class DocumentLoaderReadOnlyEventStoreRepository(IDocumentLoaderDocumentSessionProvider documentSessionProvider) 
    : Module.Sign.SharedKernel.DataAccess.ReadOnlyEventStoreRepository(documentSessionProvider), IDocumentLoaderReadOnlyEventStoreRepository
{
    
}