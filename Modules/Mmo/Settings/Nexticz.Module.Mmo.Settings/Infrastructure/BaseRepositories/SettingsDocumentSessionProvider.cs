using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.BaseRepositories;

internal class SettingsDocumentSessionProvider(
    ISettingsDocumentStore store) 
    : DocumentSessionProvider(store), ISettingsDocumentSessionProvider
{
    
}