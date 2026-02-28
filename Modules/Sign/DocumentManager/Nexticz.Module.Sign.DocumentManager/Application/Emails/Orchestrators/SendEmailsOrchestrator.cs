using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.Emails;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.Emails;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.CanUserManageLoadingDocument;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentsByCodes;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.EmailPublishers;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.Emails.Orchestrators;

internal class SendEmailsOrchestrator(
    ICurrentUserProvider currentUserProvider,
    ISender sender,
    ILogger<SendEmailsOrchestrator> logger,
    IDocumentManagerEmailPublisher emailPublisher) : ISendEmailsOrchestrator
{
    public async Task<ErrorOr<Success>> OrchestrateAsync(string[] recipients, SendEmailJobContract[] sendEmailJobContracts,
        CancellationToken cancellationToken)
    {
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        
        var validationResult = await ValidateAsync(currentUserName, recipients, sendEmailJobContracts, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var documentEmailMessages = ComposeDocumentEmailMessages(validationResult.Value.LoadingDocuments, sendEmailJobContracts);;
        
        await emailPublisher.PublishEmailsAsync(validationResult.Value.Recipients, documentEmailMessages, cancellationToken);
        
        return Result.Success;
    }
 
    private static DocumentEmailMessage[] ComposeDocumentEmailMessages(
        LoadingDocument[] loadingDocuments, SendEmailJobContract[] sendEmailJobContracts)
    {
        var documentEmailMessages = new List<DocumentEmailMessage>();
        foreach (var loadingDocument in loadingDocuments)
        {
            var shouldLoadingDocumentBeSent = 
                sendEmailJobContracts
                    .FirstOrDefault(x => 
                        x.LoadingDocumentCode == loadingDocument.Code && x.DeliveryDocumentCode is null) is not null;

            var deliveryDocumentCodes =
                sendEmailJobContracts
                    .Where(x =>
                        x.LoadingDocumentCode == loadingDocument.Code && x.DeliveryDocumentCode is not null)
                    .Select(x => x.DeliveryDocumentCode!)
                    .ToArray();
            
            documentEmailMessages.Add(new DocumentEmailMessage(loadingDocument, shouldLoadingDocumentBeSent, deliveryDocumentCodes));
        }
        
        return documentEmailMessages.ToArray();
    }
    
    private async Task<ErrorOr<ValidationResult>> ValidateAsync(string currentUserName, string[] recipients, SendEmailJobContract[] sendEmailJobContracts,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot send emails. CurrentUserName: {CurrentUserName}.",
                currentUserName);
            return EmailErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        foreach (var recipient in recipients)
        {
            if (EmailValidator.IsValidEmail(recipient)) 
                continue;
            
            logger.LogWarning(
                "SIGN - DocumentManager - not all provided users are valid. Cannot send emails. Not valid user: {InvalidEmail}, currentUserName: {CurrentUserName}.",
                recipient, currentUserName);
            return EmailErrors.ValidationInvalidEmailAddress(recipient);
        }
        
        var loadingDocumentCodes = sendEmailJobContracts
            .Select(x => x.LoadingDocumentCode)
            .Distinct()
            .ToArray();
        var loadingDocuments = await sender.Send(new GetLoadingDocumentsByCodesQuery(loadingDocumentCodes), cancellationToken);

        var userDepositorCodes = await sender.Send(
            new GetDepositorResponsesByCodesAndGroupCodesQuery(user.Value.DepositorCodes, user.Value.DepositorGroupCodes), 
            cancellationToken);
        
        foreach (var sendEmailJob in sendEmailJobContracts)
        {
            var loadingDocument = loadingDocuments.FirstOrDefault(x => x.Code == sendEmailJob.LoadingDocumentCode);
            if (loadingDocument is null)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - loading document with code: {LoadingDocumentCode} does not exist. Cannot send emails. Current user: {UserName}.",
                    sendEmailJob.LoadingDocumentCode, user.Value.UserName);
                return EmailErrors.ValidationLoadingDocumentDoesNotExists(sendEmailJob.LoadingDocumentCode);
            }
            
            if (string.IsNullOrWhiteSpace(sendEmailJob.DeliveryDocumentCode) && !loadingDocument.IsFinished)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - loading document with code: {LoadingDocumentCode} is not finished. Cannot send emails. Current user: {UserName}.",
                    sendEmailJob.LoadingDocumentCode, user.Value.UserName);
                return EmailErrors.ValidationLoadingDocumentIsNotFinished(loadingDocument.Code);
            }
            
            var canUserManageLoadingDocument = await sender.Send(new CanUserManageLoadingDocumentQuery(loadingDocument, userDepositorCodes.Select(x => x.Code).ToArray()), cancellationToken);
            if (!canUserManageLoadingDocument)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - current user: {UserName} cannot manage loading document with code: {LoadingDocumentCode}. Cannot send emails.",
                    user.Value.UserName, loadingDocument.Code);
                return EmailErrors.ValidationUserCannotManageLoadingDocument(loadingDocument.Code);
            }
            
            if (string.IsNullOrWhiteSpace(sendEmailJob.DeliveryDocumentCode))
                continue;
            
            var deliveryDocument = loadingDocument.DeliveryDocuments
                .FirstOrDefault(x => x.Code == sendEmailJob.DeliveryDocumentCode);

            if (deliveryDocument is null)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - delivery document with loading document code: {LoadingDocumentCode} " +
                    "and delivery document code: {DeliveryDocumentCode} does not exist. " +
                    "Cannot send emails. Current user: {UserName}.",
                    sendEmailJob.LoadingDocumentCode, sendEmailJob.DeliveryDocumentCode, user.Value.UserName);
                return EmailErrors.ValidationDeliveryDocumentDoNotExists(
                    loadingDocument.Code, sendEmailJob.DeliveryDocumentCode);
            }
            
            if (!deliveryDocument.IsFinished)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - delivery document with loading document code: {LoadingDocumentCode} " +
                    "and delivery document code: {DeliveryDocumentCode} is not finished. " +
                    "Cannot send emails. Current user: {UserName}.",
                    sendEmailJob.LoadingDocumentCode, sendEmailJob.DeliveryDocumentCode, user.Value.UserName);
                return EmailErrors.ValidationDocumentIsNotFinished(loadingDocument.Code, deliveryDocument.Code);
            }
        }
        
        return new ValidationResult(recipients, loadingDocuments);
    }

    private record ValidationResult(string[] Recipients, LoadingDocument[] LoadingDocuments);
}