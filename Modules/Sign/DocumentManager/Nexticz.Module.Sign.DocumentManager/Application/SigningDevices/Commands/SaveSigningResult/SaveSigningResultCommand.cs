using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SaveSigningResult;

internal record SaveSigningResultCommand(
    Guid SigningDeviceId,
    string SigningDeviceCode,
    SigningResult SigningResult, 
    DateTimeOffset ExecutedAt,
    string SentToDeviceByUserName,
    string ExecutedByDriverName,
    SentLoadingDocument[] SentLoadingDocuments
    ) : IDocumentManagerCommand<ErrorOr<Success>>;