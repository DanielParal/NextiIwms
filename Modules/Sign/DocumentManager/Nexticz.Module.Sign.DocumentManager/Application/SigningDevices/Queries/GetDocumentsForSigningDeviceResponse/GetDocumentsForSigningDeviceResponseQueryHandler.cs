using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.CanUserManageSigningDevice;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetDocumentsForSigningDeviceResponse;

internal class GetDocumentsForSigningDeviceResponseQueryHandler(
    ILogger<GetDocumentsForSigningDeviceResponseQueryHandler> logger,
    ISender sender,
    ICurrentUserProvider currentUserProvider) : IRequestHandler<GetDocumentsForSigningDeviceResponseQuery, ErrorOr<DocumentsForSigningDeviceResponse>>
{
    public async Task<ErrorOr<DocumentsForSigningDeviceResponse>> Handle(GetDocumentsForSigningDeviceResponseQuery request, CancellationToken cancellationToken)
    {
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        
        if (string.IsNullOrWhiteSpace(request.SigningDeviceCode))
        {
            logger.LogWarning(
                "SIGN - DocumentManager - signing device code is required. Cannot get documents for signing device. Current user: {UserName}.",
                currentUserName);
            return SigningDeviceErrors.ValidationSigningDeviceCodeIsRequired;
        }
        
        var upperSigningDeviceCode = request.SigningDeviceCode.ToUpperInvariant();
        
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot get documents for signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationCurrentUserNameIsNotPresentInUsers;
        }
        
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(upperSigningDeviceCode), cancellationToken);
        if (signingDevice.IsError || signingDevice.Value.IsActive == false)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - singing device does not exist or is inactive. Cannot get documents for signing device. Current user: {UserName}, signing device code: {SigningDeviceCode}.",
                currentUserName, upperSigningDeviceCode);
            return SigningDeviceErrors.ValidationSigningDeviceWithCodeDoesNotExist;
        }
        
        var canUserManageSigningDevice = await sender.Send(new CanUserManageSigningDeviceQuery(signingDevice.Value, user.Value), cancellationToken);

        if (!canUserManageSigningDevice)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user: {UserName} cannot manage signing device with code: {DeviceCode}. Cannot get documents for signing device.",
                currentUserName, signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationCurrentUserCannotManageSigningDevice;
        }
        
        return DocumentsForSigningDeviceResponseFactory.Create(signingDevice.Value.SentDocuments);
    }
}