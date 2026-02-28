using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.CreateUser;

internal class CreateUserCommandHandler(
    ISender sender,
    ISettingsUnitOfWork unitOfWork,
    ILogger<CreateUserCommandHandler> logger) : IRequestHandler<CreateUserCommand, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (existingUser.HasValue())
        {
            logger.LogWarning("Object {ObjectName} with username: {UserName} already exists. Nothing to create.", nameof(User), request.UserName);
            return UserErrors.ValidationUserWithUserNameAlreadyExist;
        }
        
        var user = new User(request.UserName, true, request.Roles, request.Permissions, []);
        var userCreatedEvent = new UserCreatedEvent(user.Id, user.UserName, user.IsActive, user.Roles, user.Permissions, []);
        unitOfWork
            .StartStream<UserCreatedEvent, User>(
                user.Id, userCreatedEvent);
        
        return user;
    }
}