using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

public record DocumentsFromSigningDeviceReturnedEvent(
    Guid Id,
    string Code,
    DateTimeOffset ReturnedAt,
    string ReturnedByUserName,
    SentLoadingDocument[] SentLoadingDocuments) : IMartenEvent;