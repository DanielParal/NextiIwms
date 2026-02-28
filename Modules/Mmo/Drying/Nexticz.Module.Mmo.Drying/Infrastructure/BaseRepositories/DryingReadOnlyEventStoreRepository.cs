using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Drying.Infrastructure.BaseRepositories;

internal class DryingReadOnlyEventStoreRepository(IDryingDocumentSessionProvider documentSessionProvider) 
    : ReadOnlyEventStoreRepository(documentSessionProvider), IDryingReadOnlyEventStoreRepository;