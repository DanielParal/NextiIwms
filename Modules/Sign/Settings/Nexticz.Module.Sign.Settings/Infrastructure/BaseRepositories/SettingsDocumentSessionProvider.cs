using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.SharedKernel.DataAccess;

namespace Nexticz.Module.Sign.Settings.Infrastructure.BaseRepositories;

internal class SettingsDocumentSessionProvider(
    ISettingsDocumentStore store) 
    : DocumentSessionProvider(store), ISettingsDocumentSessionProvider
{
    
}