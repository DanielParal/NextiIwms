using Marten;

namespace Nexticz.Module.Mmo.Washing.Application.Interfaces;

internal interface IGlobalKitsCounter
{
    Task InitializeAsync(IDocumentSession session, CancellationToken cancellationToken);
    int GetAndIncrementCurrentValue();
}