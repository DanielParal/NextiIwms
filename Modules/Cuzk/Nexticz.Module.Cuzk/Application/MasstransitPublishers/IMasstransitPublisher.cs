using Nexticz.Lib.Shared.MessagePublishers;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.MasstransitPublishers;

internal interface IMasstransitPublisher : IBaseMessagePublisher
{
    Task NotifyImportFinishedAsync(Guid importId, ImportType importType, string requestedByUserName, CancellationToken cancellationToken);

    Task NotifyImportFailedAsync(Guid importId, ImportType importType, string requestedByUserName,
        string errorDescription, string logId, CancellationToken cancellationToken);
}