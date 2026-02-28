using Nexticz.Module.Sign.DocumentLoader.Application.Interfaces;

namespace Nexticz.Module.Sign.DocumentLoader.Infrastructure.BaseRepositories;

internal class DocumentLoaderDocumentSessionProvider(
    IDocumentLoaderDocumentStore store) 
    : Module.Sign.SharedKernel.DataAccess.DocumentSessionProvider(store), IDocumentLoaderDocumentSessionProvider
{
    
}