using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.Cuzk.Application.Interfaces;

namespace Nexticz.Module.Cuzk.Infrastructure.BaseRepositories;

internal class CuzkReadOnlyEventStoreRepository(ICuzkDocumentSessionProvider documentSessionProvider) 
    : MartenReadOnlyEventStoreRepository(documentSessionProvider), ICuzkReadOnlyEventStoreRepository
{
    
}