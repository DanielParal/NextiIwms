using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.NotificationCollectors;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersBySigningDeviceCode;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.DeleteSigningDevice;

internal class DeleteSigningDeviceCommandHandler(
    ILogger<DeleteSigningDeviceCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork,
    ISettingsNotificationCollector notificationCollector
) 
    : IRequestHandler<DeleteSigningDeviceCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteSigningDeviceCommand request, CancellationToken cancellationToken)
    {
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(request.Code), cancellationToken);

        if (signingDevice.IsError)
        {
            logger.LogInformation("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to delete.", 
                nameof(SigningDevice), request.Code);
            return SigningDeviceErrors.ValidationCodeDoesNotExist;
        }
        
        var users = await sender.Send(new GetUsersBySigningDeviceCodeQuery(signingDevice.Value.Code), cancellationToken);
        if (users.Length > 0)
        {
            var userNamesString = string.Join(", ", users.Select(x => x.UserName));
            logger.LogInformation("Sign - we cannot delete {ObjectName} with code: {Code} because it is assigned to users: {userNamesString}.",
                nameof(SigningDevice), signingDevice.Value.Code, userNamesString);
            return SigningDeviceErrors.ValidationCodeIsAssignedToUsers(userNamesString);
        }
        
        var signingDeviceDeletedEvent = new SigningDeviceDeletedEvent(signingDevice.Value.Id, request.Code);
        unitOfWork.AppendEvent(signingDevice.Value.Id, signingDeviceDeletedEvent);
        
        notificationCollector.AddNotification(new SigningDeviceDeletedNotification(signingDevice.Value.Code));
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} deleted.",
            nameof(SigningDevice), request.Code);
        return Result.Deleted;
    }
}