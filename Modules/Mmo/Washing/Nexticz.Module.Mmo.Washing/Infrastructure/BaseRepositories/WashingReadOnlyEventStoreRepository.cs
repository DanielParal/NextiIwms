using Marten;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.BaseRepositories;

internal class WashingReadOnlyEventStoreRepository(IWashingDocumentSessionProvider documentSessionProvider) 
    : Module.Mmo.SharedKernel.DataAccess.ReadOnlyEventStoreRepository(documentSessionProvider), IWashingReadOnlyEventStoreRepository;