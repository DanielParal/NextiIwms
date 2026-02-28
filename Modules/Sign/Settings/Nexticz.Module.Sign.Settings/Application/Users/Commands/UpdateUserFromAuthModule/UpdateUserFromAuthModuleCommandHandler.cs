using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel.Security;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.UpdateUserFromAuthModule;

internal class UpdateUserFromAuthModuleCommandHandler(
    ILogger<UpdateUserFromAuthModuleCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<UpdateUserFromAuthModuleCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserFromAuthModuleCommand request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (!user.HasValue())
        {
            logger.LogWarning("Sign - Object {ObjectName} with username: {UserName} does not exists. " +
                              "Nothing to update from auth module.", nameof(User), request.UserName);
            return UserErrors.ValidationUserNameDoesNotExist;
        }
        
        var isActive = request.Roles.Any(x => RoleHelper.GetNames().Contains(x));
        
        var userUpdatedEvent = new UserUpdatedFromAuthModuleEvent(user.Value.Id, user.Value.UserName, request.FullName, isActive, request.Roles, request.Permissions);
        unitOfWork
            .AppendEvent(
                user.Value.Id, userUpdatedEvent);
        
        return Result.Updated;
    }
}