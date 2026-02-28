using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Interfaces;

internal interface ILastEnteredWorkerOnLineRepository
{
    Task<LastEnteredWorkerOnLine?> GetLastEnteredWorkerOnLineAsync(string lineCode, CancellationToken cancellationToken);
}