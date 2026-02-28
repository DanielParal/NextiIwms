namespace Nexticz.Module.Sign.DocumentManager.Application.Interfaces;

internal interface IDocumentManagerUnitOfWork : Module.Sign.SharedKernel.DataAccess.IUnitOfWork
{
    Task<bool> RebuildProjectionAsync(string projectionTypeString, CancellationToken cancellationToken);
}