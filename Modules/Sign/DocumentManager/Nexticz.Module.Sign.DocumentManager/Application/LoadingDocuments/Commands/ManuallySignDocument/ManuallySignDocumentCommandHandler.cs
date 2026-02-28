using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.CanUserManageLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.ManuallySignDocument;

internal class ManuallySignDocumentCommandHandler(
    ILogger<ManuallySignDocumentCommandHandler> logger,
    ISender sender,
    ICurrentUserProvider currentUserProvider,
    IDocumentManagerUnitOfWork unitOfWork,
    IDocumentManagerFileHandler fileHandler,
    IClock clock) : IRequestHandler<ManuallySignDocumentCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ManuallySignDocumentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        await fileHandler.CopyDocumentFilesFromManagerToHistorySourceFilesFolderAsync(request.LoadingDocumentCode, request.DeliveryDocumentCode, cancellationToken);
        await fileHandler.SaveSignedDocumentAsync(request.FormFile, request.LoadingDocumentCode, request.DeliveryDocumentCode, cancellationToken);

        var isLoadingDocumentSigned = string.IsNullOrWhiteSpace(request.DeliveryDocumentCode);
        var signedAt = clock.TenantNowOffset;
        var depositorName = await GetDepositorNameAsync(validationResult.Value.LoadingDocument.DepositorCode, cancellationToken);
        
        validationResult.Value.LoadingDocument.ManuallySignDocumentWithUploadedImage(isLoadingDocumentSigned, request.DeliveryDocumentCode, signedAt, validationResult.Value.UserName, validationResult.Value.UserFullName, depositorName);
        
        var documentManuallySignedEvent = new SignedDocumentManuallyUploadedEvent(
            validationResult.Value.LoadingDocument.Id,
            validationResult.Value.LoadingDocument.Code,
            request.DeliveryDocumentCode,
            isLoadingDocumentSigned,
            signedAt,
            validationResult.Value.UserName,
            validationResult.Value.UserFullName,
            depositorName);
        
        unitOfWork.AppendEvent(validationResult.Value.LoadingDocument.Id, documentManuallySignedEvent);
        
        if (validationResult.Value.LoadingDocument.IsLoadingDocumentFullyFinishedWithAllDeliveryDocuments())
        {
            var loadingDocumentFullyFinishedEvent = new LoadingDocumentWithAllDeliveryDocumentsFinishedEvent(
                validationResult.Value.LoadingDocument.Id, validationResult.Value.LoadingDocument.Code);
            unitOfWork.AppendEvent(validationResult.Value.LoadingDocument.Id, loadingDocumentFullyFinishedEvent);
            
            logger.LogInformation("SIGN - DocumentManager - loading document is fully finished with all delivery documents after manual signing document. LoadingDocumentId: {LoadingDocumentId}, LoadingDocumentCode: {LoadingDocumentCode}.", 
                validationResult.Value.LoadingDocument.Id, validationResult.Value.LoadingDocument.Code);
        }
        
        return Result.Success;
    }

    private async Task<ErrorOr<ValidationResult>> ValidateAsync(ManuallySignDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        
        var isFileValid = fileHandler.IsFileValid(request.FormFile, 5* 1024 * 1024, [".pdf"]);
        if (isFileValid.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - file is not valid. Cannot manually sign document. Current user: {UserName}, loading document code: {LoadingDocumentCode}, " +
                "delivery document code: {DeliveryDocumentCode}, file error code: {DomainErrorCode}, file error message: {DomainErrorMessage}.",
                currentUserName, request.LoadingDocumentCode, request.DeliveryDocumentCode, isFileValid.FirstError.Code, isFileValid.FirstError.Description);
            return isFileValid.Errors;
        }
        
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot manually sign loading document. Current user: {UserName}, loading document code: {LoadingDocumentCode}, delivery document code: {DeliveryDocumentCode}.",
                currentUserName, request.LoadingDocumentCode, request.DeliveryDocumentCode);
            return LoadingDocumentErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        var loadingDocument = await sender.Send(new GetLoadingDocumentByCodeQuery(request.LoadingDocumentCode), cancellationToken);
        if (loadingDocument.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - object {ObjectName} does not exist. Cannot manually sign loading document. Current user: {UserName}, loading document code: {LoadingDocumentCode}, delivery document code: {DeliveryDocumentCode}.",
                nameof(LoadingDocument) ,currentUserName, request.LoadingDocumentCode, request.DeliveryDocumentCode);
            return LoadingDocumentErrors.ValidationLoadingDocumentDoesNotExist;
        }
        
        var canDocumentBeSigned = loadingDocument.Value.CanDocumentBeManuallySigned(request.DeliveryDocumentCode);
        if (canDocumentBeSigned.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - object {ObjectName} cannot be manually signed. Current user: {UserName}, loading document code: {LoadingDocumentCode}, " +
                "delivery document code: {DeliveryDocumentCode}, Domain error code: {DomainErrorCode}, Domain error message: {DomainErrorMessage}.",
                nameof(LoadingDocument) ,currentUserName, request.LoadingDocumentCode, request.DeliveryDocumentCode, canDocumentBeSigned.FirstError.Code, canDocumentBeSigned.FirstError.Description);
            return canDocumentBeSigned.Errors;
        }
        
        var depositorCodes = await sender.Send(
            new GetDepositorResponsesByCodesAndGroupCodesQuery(user.Value.DepositorCodes, user.Value.DepositorGroupCodes), 
            cancellationToken);
        
        var canUserManageLoadingDocument = await sender.Send(new CanUserManageLoadingDocumentQuery(loadingDocument.Value, depositorCodes.Select(dc => dc.Code).ToArray()), cancellationToken);
        if (!canUserManageLoadingDocument)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user: {UserName} cannot manage loading document with code: {LoadingDocumentCode}. We cannot manually sign document.",
                user.Value.UserName, loadingDocument.Value.Code);
            return LoadingDocumentErrors.ValidationCurrentUserCannotManageLoadingDocument;
        };
        
        return new ValidationResult(user.Value.UserName, user.Value.FullName, loadingDocument.Value);
    }

    private async Task<string?> GetDepositorNameAsync(string depositorCode, CancellationToken cancellationToken)
    {
        var depositorResponse = await sender.Send(new GetDepositorResponseByCodeQuery(depositorCode), cancellationToken);
        return depositorResponse.IsError ? null : depositorResponse.Value.Name;
    }
    
    private record ValidationResult(string UserName, string? UserFullName, LoadingDocument LoadingDocument);
}