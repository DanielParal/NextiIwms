
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentFromSigningDeviceReturnedEvent(
    Guid Id,
    string LoadingDocumentCode,
    bool IsLoadingDocumentAlsoReturned,
    string[] DeliveryDocumentCodes,
    string SigningDeviceCode,
    DateTimeOffset ReturnedAt,
    string ReturnedByUserName) : IMartenEvent;