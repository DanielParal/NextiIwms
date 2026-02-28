using Nexticz.Module.Sign.Settings.Application.Interfaces;

namespace Nexticz.Module.Sign.Settings.Infrastructure.BaseRepositories;

internal class SettingsReadOnlyEventStoreRepository(ISettingsDocumentSessionProvider documentSessionProvider) 
    : Module.Sign.SharedKernel.DataAccess.ReadOnlyEventStoreRepository(documentSessionProvider), ISettingsReadOnlyEventStoreRepository
{
    
}