using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserById;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.UpdateUser;

internal class UpdateUserCommandHandler(
    ISender sender,
    ILogger<UpdateUserCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByIdQuery(request.Id), cancellationToken);

        if (user.IsError)
        {
            logger.LogWarning("Object {ObjectName} with id: {Id} does not exist. Nothing to update.", nameof(User), request.Id);
            return UserErrors.ValidationUserWithUserNameDoesNotExist;
        }
        
        var userUpdatedEvent = new UserUpdatedEvent(user.Value.Id, request.ReceivableNotifications);
        unitOfWork.AppendEvent(user.Value.Id, userUpdatedEvent);

        return Result.Updated;
    }
}