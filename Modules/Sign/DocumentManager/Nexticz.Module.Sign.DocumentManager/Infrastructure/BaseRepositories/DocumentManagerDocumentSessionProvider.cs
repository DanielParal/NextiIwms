using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.BaseRepositories;

internal class DocumentManagerDocumentSessionProvider(
    IDocumentManagerDocumentStore store) 
    : Module.Sign.SharedKernel.DataAccess.DocumentSessionProvider(store), IDocumentManagerDocumentSessionProvider
{
    
}