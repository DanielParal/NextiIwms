using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.CanUserManageSigningDevice;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetDocumentFileForSigningDevice;

internal class GetDocumentFileForSigningDeviceQueryHandler(
    ILogger<GetDocumentFileForSigningDeviceQueryHandler> logger,
    ISender sender,
    ICurrentUserProvider currentUserProvider,
    IDocumentManagerFileHandler fileHandler) 
    : IRequestHandler<GetDocumentFileForSigningDeviceQuery, ErrorOr<FileResult>>
{
    public async Task<ErrorOr<FileResult>> Handle(GetDocumentFileForSigningDeviceQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var findFileResult = 
            FindFileOnDevice(
                validationResult.Value.DocumentCode, 
                validationResult.Value.SentLoadingDocuments, 
                validationResult.Value.CurrentUserName, 
                validationResult.Value.SigningDeviceCode);
        
        if (findFileResult.IsError)
            return findFileResult.Errors;
        
        var file = await fileHandler.GetDocumentFileFromManagerAsync(findFileResult.Value.FolderName, findFileResult.Value.FileName, cancellationToken);

        if (file is null)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - cannot get file for signing device because file is not present. " +
                "Current user: {UserName}, signing device code: {SigningDeviceCode}, provided document code: {DocumentCode}.",
                validationResult.Value.CurrentUserName, validationResult.Value.SigningDeviceCode, validationResult.Value.DocumentCode);
            return SigningDeviceErrors.ValidationFileIsNotFound;
        }

        return file;
    }

    private ErrorOr<(string FolderName, string FileName)> FindFileOnDevice(
        string documentCode, SentLoadingDocument[] sentDocuments,
        string currentUserName, string signingDeviceCode)
    {
        var loadingDocument = sentDocuments.FirstOrDefault(x => x.LoadingDocumentCode == documentCode);

        if (loadingDocument is not null)
            return (loadingDocument.LoadingDocumentCode, GetFileName(loadingDocument.LoadingDocumentCode));
        
        loadingDocument = sentDocuments.FirstOrDefault(x => x.DeliveryDocuments.Select(dd => dd.Code).ToArray().Contains(documentCode));

        if (loadingDocument is null)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - cannot get file for signing device because document is not on the device. " +
                "Current user: {UserName}, signing device code: {SigningDeviceCode}, provided document code: {DocumentCode}.",
                currentUserName, signingDeviceCode, documentCode);
            return SigningDeviceErrors.ValidationDocumentCodeIsNotOnTheDevice;
        }
        
        return (loadingDocument.LoadingDocumentCode, GetFileName(documentCode));
    }

    private static string GetFileName(string documentCode) => $"{documentCode}.pdf";

    private async Task<ErrorOr<ValidationResult>> ValidateAsync(GetDocumentFileForSigningDeviceQuery request, CancellationToken cancellationToken)
    {
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        
        if (string.IsNullOrWhiteSpace(request.SigningDeviceCode))
        {
            logger.LogWarning(
                "SIGN - DocumentManager - signing device code is required. Cannot get file for signing device. Current user: {UserName}.",
                currentUserName);
            return SigningDeviceErrors.ValidationSigningDeviceCodeIsRequired;
        }
        
        var upperSigningDeviceCode = request.SigningDeviceCode.ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(request.DocumentCode))
        {
            logger.LogWarning(
                "SIGN - DocumentManager - document code is required. Cannot get file for signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationDocumentCodeIsRequired;
        }
        
        var upperDocumentCode = request.DocumentCode.ToUpperInvariant();
        
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot get file for signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(upperSigningDeviceCode), cancellationToken);
        if (signingDevice.IsError || signingDevice.Value.IsActive == false)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - singing device does not exist or is inactive. Cannot get file for signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationSigningDeviceWithCodeDoesNotExist;
        }
        
        var canUserManageSigningDevice = await sender.Send(new CanUserManageSigningDeviceQuery(signingDevice.Value, user.Value), cancellationToken);

        if (!canUserManageSigningDevice)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user: {UserName} cannot manage signing device with code: {SigningDeviceCode}. Cannot get file for signing device.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationCurrentUserCannotManageSigningDevice;
        }
        
        return new ValidationResult(upperDocumentCode, upperSigningDeviceCode, currentUserName, signingDevice.Value.SentDocuments);
    }
    
    private record ValidationResult(string DocumentCode, string SigningDeviceCode, string CurrentUserName, SentLoadingDocument[] SentLoadingDocuments);
}