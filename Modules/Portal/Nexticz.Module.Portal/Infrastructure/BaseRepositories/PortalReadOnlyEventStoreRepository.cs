using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.Portal.Application.Interfaces;

namespace Nexticz.Module.Portal.Infrastructure.BaseRepositories;

internal class PortalReadOnlyEventStoreRepository(IPortalDocumentSessionProvider documentSessionProvider) 
    : MartenReadOnlyEventStoreRepository(documentSessionProvider), IPortalReadOnlyEventStoreRepository
{
    
}