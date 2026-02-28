namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface ISettingsUnitOfWork : Module.Mmo.SharedKernel.DataAccess.IUnitOfWork
{
    Task<bool> RebuildProjectionAsync(string projectionTypeString, CancellationToken cancellationToken);
}