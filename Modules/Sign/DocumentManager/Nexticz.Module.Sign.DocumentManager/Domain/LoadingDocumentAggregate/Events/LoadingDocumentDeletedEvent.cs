using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentDeletedEvent(
    Guid Id,
    string Code,
    bool IsLoadingDocumentAlsoDeleted,
    string[] DeliveryDocumentCodes,
    DateTimeOffset DeletedAt,
    string DeletedByUserName,
    string? DeletedByUserFullName,
    string? DepositorName,
    string? DeletionReason) : IMartenEvent;