using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Drying.Infrastructure.BaseRepositories;

internal class DryingDocumentSessionProvider(
    IDryingDocumentStore store) 
    : DocumentSessionProvider(store), IDryingDocumentSessionProvider
{
    
}