using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.BaseRepositories;

internal class SettingsUnitOfWork(ISettingsDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : Module.Mmo.SharedKernel.DataAccess.UnitOfWork(documentSessionProvider, currentUserProvider), ISettingsUnitOfWork
{
    public async Task<bool> RebuildProjectionAsync(string projectionTypeString, CancellationToken cancellationToken)
    {
        return await MartenRebuilder.RebuildProjectionAsync(
            documentSessionProvider.GetStore(), typeof(ISettingsDocumentStore).Assembly, projectionTypeString, cancellationToken);
    }
}