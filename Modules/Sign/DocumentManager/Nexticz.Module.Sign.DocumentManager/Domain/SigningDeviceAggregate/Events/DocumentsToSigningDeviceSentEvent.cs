using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

public record DocumentsToSigningDeviceSentEvent(
    Guid Id,
    string Code,
    DateTimeOffset SentAt,
    string SentByUserName,
    SentLoadingDocument[] SentLoadingDocuments) : IMartenEvent;