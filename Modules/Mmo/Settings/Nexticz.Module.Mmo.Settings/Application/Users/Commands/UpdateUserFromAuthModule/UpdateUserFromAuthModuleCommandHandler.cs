using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;
using Nexticz.Module.Mmo.SharedKernel.Security;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.UpdateUserFromAuthModule;

internal class UpdateUserFromAuthModuleCommandHandler(
    ISender sender, ISettingsUnitOfWork unitOfWork, ILogger<UpdateUserFromAuthModuleCommandHandler> logger)
    : IRequestHandler<UpdateUserFromAuthModuleCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserFromAuthModuleCommand request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (user.IsError)
        {
            logger.LogWarning("Object {ObjectName} with username: {UserName} does not exists. Nothing to update from auth module.", nameof(User), request.UserName);
            return UserErrors.ValidationUserWithUserNameDoesNotExist;
        }
        
        var isActive = request.Roles.Any(x => RoleHelper.GetNames().Contains(x));
        
        var userUpdatedEvent = new UserUpdatedFromAuthModuleEvent(user.Value.Id, isActive, request.Roles, request.Permissions);
        unitOfWork
            .AppendEvent(
                user.Value.Id, userUpdatedEvent);
        
        return Result.Updated;
    }
}