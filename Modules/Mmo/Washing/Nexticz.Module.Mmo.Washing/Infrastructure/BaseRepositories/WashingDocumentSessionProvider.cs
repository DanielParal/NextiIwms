using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.BaseRepositories;

internal class WashingDocumentSessionProvider(
    IWashingDocumentStore store) 
    : DocumentSessionProvider(store), IWashingDocumentSessionProvider
{
    
}