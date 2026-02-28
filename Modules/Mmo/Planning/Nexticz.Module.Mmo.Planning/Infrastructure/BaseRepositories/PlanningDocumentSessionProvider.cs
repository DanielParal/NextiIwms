using Marten;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;

namespace Nexticz.Module.Mmo.Planning.Infrastructure.BaseRepositories;

internal class PlanningDocumentSessionProvider(
    IPlanningDocumentStore store) 
    : DocumentSessionProvider(store), IPlanningDocumentSessionProvider
{
    
}