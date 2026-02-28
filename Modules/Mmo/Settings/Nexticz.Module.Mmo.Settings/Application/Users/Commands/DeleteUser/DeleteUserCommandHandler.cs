using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.DeleteUser;

internal class DeleteUserCommandHandler(
    ISender sender, ISettingsUnitOfWork unitOfWork, ILogger<DeleteUserCommandHandler> logger)
    : IRequestHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (user.IsError)
        {
            logger.LogWarning("Object {ObjectName} with username: {UserName} does not exists. Nothing to delete.", nameof(User), request.UserName);
            return UserErrors.ValidationUserWithUserNameDoesNotExist;
        }

        var userDeletedEvent = new UserDeletedEvent(user.Value.Id, user.Value.UserName);
        unitOfWork
            .AppendEvent(
                user.Value.Id, userDeletedEvent);
        
        return Result.Deleted;
    }
}