using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.CanUserManageLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.ChangePrintCopiesCount;

internal class ChangePrintCopiesCountCommandHandler(
    ILogger<ChangePrintCopiesCountCommandHandler> logger,
    ISender sender,
    ICurrentUserProvider currentUserProvider,
    IDocumentManagerUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<ChangePrintCopiesCountCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ChangePrintCopiesCountCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var result = validationResult.Value.LoadingDocument.ChangePrintCopiesCount(request.DeliveryDocumentCode, request.PrintCopiesCount);
        if (result.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - Cannot change print copies count. " +
                "LoadingDocumentCode: {LoadingDocumentCode}, DeliveryDocumentCode: {DeliveryDocumentCode}, " +
                "CurrentUserName: {CurrentUserName}, PrintCopiesCount: {PrintCopiesCount}, " +
                "ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}.",
                request.LoadingDocumentCode, request.DeliveryDocumentCode, validationResult.Value.UserName, request.PrintCopiesCount,
                result.FirstError.Code, result.FirstError.Description);
            return result.Errors;
        }
        
        var printCopiesCountChangedEvent = new LoadingDocumentPrintCopiesCountChangedEvent(
            validationResult.Value.LoadingDocument.Id,
            validationResult.Value.LoadingDocument.Code,
            request.DeliveryDocumentCode,
            request.PrintCopiesCount,
            clock.UtcNowOffset,
            validationResult.Value.UserName);
        
        unitOfWork.AppendEvent(validationResult.Value.LoadingDocument.Id, printCopiesCountChangedEvent);
        
        logger.LogInformation(
            "SIGN - DocumentManager - copies count for loading document changed. " +
            "LoadingDocumentCode: {LoadingDocumentCode}, DeliveryDocumentCode: {DeliveryDocumentCode}, " +
            "CurrentUserName: {CurrentUserName}, PrintCopiesCount: {PrintCopiesCount}.",
            request.LoadingDocumentCode, request.DeliveryDocumentCode, validationResult.Value.UserName, request.PrintCopiesCount);
        
        return Result.Success;
    }

    private async Task<ErrorOr<ValidationResult>> ValidateAsync(ChangePrintCopiesCountCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot change print copies count. Current user: {UserName}, loading document code: {LoadingDocumentCode}, delivery document code: {DeliveryDocumentCode}.",
                currentUserName, request.LoadingDocumentCode, request.DeliveryDocumentCode);
            return LoadingDocumentErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        var loadingDocument = await sender.Send(new GetLoadingDocumentByCodeQuery(request.LoadingDocumentCode), cancellationToken);
        if (loadingDocument.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - object {ObjectName} does not exist. Cannot change print copies count. Current user: {UserName}, loading document code: {LoadingDocumentCode}, delivery document code: {DeliveryDocumentCode}.",
                nameof(LoadingDocument) ,currentUserName, request.LoadingDocumentCode, request.DeliveryDocumentCode);
            return LoadingDocumentErrors.ValidationLoadingDocumentDoesNotExist;
        }
        
        var depositorCodes = await sender.Send(
            new GetDepositorResponsesByCodesAndGroupCodesQuery(user.Value.DepositorCodes, user.Value.DepositorGroupCodes), 
            cancellationToken);
        
        var canUserManageLoadingDocument = await sender.Send(new CanUserManageLoadingDocumentQuery(loadingDocument.Value, depositorCodes.Select(dc => dc.Code).ToArray()), cancellationToken);
        if (!canUserManageLoadingDocument)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user: {UserName} cannot manage loading document with code: {LoadingDocumentCode}. We cannot change print copies count.",
                user.Value.UserName, loadingDocument.Value.Code);
            return LoadingDocumentErrors.ValidationCurrentUserCannotManageLoadingDocument;
        };
        
        return new ValidationResult(user.Value.UserName, loadingDocument.Value);
    }
    
    private record ValidationResult(string UserName, LoadingDocument LoadingDocument);
}