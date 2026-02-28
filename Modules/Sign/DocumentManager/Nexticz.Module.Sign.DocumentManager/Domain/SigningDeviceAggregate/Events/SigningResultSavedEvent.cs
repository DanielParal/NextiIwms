using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

public record SigningResultSavedEvent(
    Guid Id,
    string Code,
    SigningResult SigningResult,
    DateTimeOffset ExecutedAt,
    string SentToDeviceByUserName,
    string ExecutedByUserName,
    SentLoadingDocument[] SentLoadingDocuments) : IMartenEvent;