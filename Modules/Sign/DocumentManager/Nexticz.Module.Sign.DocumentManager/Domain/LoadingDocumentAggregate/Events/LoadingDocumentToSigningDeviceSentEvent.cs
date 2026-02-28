using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentToSigningDeviceSentEvent(
    Guid Id,
    string Code,
    bool IsLoadingDocumentAlsoSent,
    string[] DeliveryDocumentCodes,
    string SigningDeviceCode,
    DateTimeOffset SentAt,
    string SentByUserName) : IMartenEvent;