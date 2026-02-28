using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.FileHandling;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.DeleteUserFromAuthModule;

internal class DeleteUserFromAuthModuleCommandHandler(
    ILogger<DeleteUserFromAuthModuleCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork,
    ISettingsFileHandler settingsFileHandler) 
    : IRequestHandler<DeleteUserFromAuthModuleCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteUserFromAuthModuleCommand request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (!user.HasValue())
        {
            logger.LogWarning("Sign - Object {ObjectName} with username: {UserName} does not exists. Nothing to delete from sign module.", 
                nameof(User), request.UserName);
            return UserErrors.ValidationUserNameDoesNotExist;
        }
        
        settingsFileHandler.DeleteUserSignature(user.Value.Id);
        
        var userDeletedEvent = new UserDeletedFromAuthModuleEvent(user.Value.Id, user.Value.UserName);
        unitOfWork
            .AppendEvent(
                user.Value.Id, userDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with username: {UserName} deleted.", 
            nameof(User), user.Value.UserName);
        
        return Result.Deleted;
    }
}