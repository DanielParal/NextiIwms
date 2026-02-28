using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.Settings.Application.Interfaces;

namespace Nexticz.Module.Sign.Settings.Infrastructure.BaseRepositories;

internal class SettingsUnitOfWork(
    ISettingsDocumentSessionProvider documentSessionProvider,
    ICurrentUserProvider currentUserProvider)
    : Module.Sign.SharedKernel.DataAccess.UnitOfWork(documentSessionProvider, currentUserProvider), ISettingsUnitOfWork
{
    public async Task<bool> RebuildProjectionAsync(string projectionTypeString, CancellationToken cancellationToken)
    {
        return await MartenRebuilder.RebuildProjectionAsync(
            documentSessionProvider.GetStore(), typeof(ISettingsDocumentStore).Assembly, projectionTypeString, cancellationToken);
    }
}