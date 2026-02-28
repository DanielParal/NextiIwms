using Marten;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceResponsesForUser;

internal class GetSigningDeviceResponsesForUserQueryHandler(
    ISender sender,
    ILogger<GetSigningDeviceResponsesForUserQueryHandler> logger,
    ICurrentUserProvider currentUserProvider,
    IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository)  : IRequestHandler<GetSigningDeviceResponsesForUserQuery, SigningDeviceResponse[]>
{
    public async Task<SigningDeviceResponse[]> Handle(GetSigningDeviceResponsesForUserQuery request, CancellationToken cancellationToken)
    {
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning(
                "SIGN - DocumentManager - current user is not present in users. Cannot get signing devices for user: {CurrentUserName}.",
                currentUserName);
            return [];
        }
        
        var userDevicesUpperCodes = user.Value.SigningDeviceCodes.Select(dc => dc.ToUpperInvariant()).ToArray();
        var signingDevices = await readOnlyEventStoreRepository.GetAllByConditionAsync<SigningDevice>(
            x => x.IsActive && x.Code.IsOneOf(userDevicesUpperCodes), cancellationToken);
        
        return signingDevices.Select(x => new SigningDeviceResponse(x.Id, x.Code, x.Name, x.SentDocuments.Length > 0)).ToArray();
    }
}