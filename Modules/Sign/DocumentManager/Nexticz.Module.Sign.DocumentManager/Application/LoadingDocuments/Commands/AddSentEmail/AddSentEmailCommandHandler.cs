using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.AddSentEmail;

internal class AddSentEmailCommandHandler(
    ILogger<AddSentEmailCommandHandler> logger,
    ISender sender,
    IDocumentManagerUnitOfWork unitOfWork) : IRequestHandler<AddSentEmailCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(AddSentEmailCommand request, CancellationToken cancellationToken)
    {
        var loadingDocument = await sender.Send(new GetLoadingDocumentByCodeQuery(request.LoadingDocumentCode), cancellationToken);

        if (loadingDocument.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - object {ObjectName} does not exist. Cannot add sent email. LoadingDocumentCode: {LoadingDocumentCode}, DeliveryDocumentCode: {DeliveryDocumentCode}, EmailId: {EmailId}.",
                nameof(LoadingDocument), request.LoadingDocumentCode, request.DeliveryDocumentCode, request.EmailId);
            return loadingDocument.Errors;
        }

        if (!string.IsNullOrWhiteSpace(request.DeliveryDocumentCode))
        {
            var deliveryDocument = loadingDocument.Value.DeliveryDocuments.FirstOrDefault(x => x.Code == request.DeliveryDocumentCode);
            if (deliveryDocument is null)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - object {ObjectName} does not exist in loading document. Cannot add sent email. LoadingDocumentCode: {LoadingDocumentCode}, DeliveryDocumentCode: {DeliveryDocumentCode}, EmailId: {EmailId}.",
                    nameof(DeliveryDocument), request.LoadingDocumentCode, request.DeliveryDocumentCode, request.EmailId);
                return LoadingDocumentErrors.ValidationDeliveryDocumentDoesNotExistInLoadingDocument(request.LoadingDocumentCode, request.DeliveryDocumentCode);
            }
        }

        if (request.Recipients.Length == 0)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - Cannot add sent email because there are no recipients. LoadingDocumentCode: {LoadingDocumentCode}, DeliveryDocumentCode: {DeliveryDocumentCode}, EmailId: {EmailId}.",
                request.LoadingDocumentCode, request.DeliveryDocumentCode, request.EmailId);
            return LoadingDocumentErrors.ValidationNoRecipientsForEmailAttempt;
        }
        
        var emailAttemptCreatedEvent = 
            new LoadingDocumentEmailSentEvent(
                loadingDocument.Value.Id, loadingDocument.Value.Code, request.DeliveryDocumentCode, request.EmailId, 
                request.Recipients, request.ProcessedAt, loadingDocument.Value.GetEmailMetadata(request.DeliveryDocumentCode), request.AttachmentsCount, request.FailureReason);

        unitOfWork.AppendEvent(loadingDocument.Value.Id, emailAttemptCreatedEvent);
        
        logger.LogInformation(
            "SIGN - DocumentManager - sent email added. LoadingDocumentCode: {LoadingDocumentCode}, DeliveryDocumentCode: {DeliveryDocumentCode}, EmailId: {EmailId}.",
            request.LoadingDocumentCode, request.DeliveryDocumentCode, request.EmailId);

        return Result.Success;
    }
}