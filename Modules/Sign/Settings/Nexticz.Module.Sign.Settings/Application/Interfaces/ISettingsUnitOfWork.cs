namespace Nexticz.Module.Sign.Settings.Application.Interfaces;

internal interface ISettingsUnitOfWork : Module.Sign.SharedKernel.DataAccess.IUnitOfWork
{
    Task<bool> RebuildProjectionAsync(string projectionTypeString, CancellationToken cancellationToken);
}