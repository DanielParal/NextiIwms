using Marten;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;

namespace Nexticz.Module.Mmo.Planning.Infrastructure.BaseRepositories;

internal class PlanningReadOnlyEventStoreRepository(IPlanningDocumentSessionProvider documentSessionProvider) 
    : Module.Mmo.SharedKernel.DataAccess.ReadOnlyEventStoreRepository(documentSessionProvider), IPlanningReadOnlyEventStoreRepository;