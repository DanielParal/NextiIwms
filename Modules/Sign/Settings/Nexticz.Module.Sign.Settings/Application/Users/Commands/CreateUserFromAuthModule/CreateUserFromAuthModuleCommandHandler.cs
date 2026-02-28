using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.CreateUserFromAuthModule;

internal class CreateUserFromAuthModuleCommandHandler(
        ILogger<CreateUserFromAuthModuleCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender
    ) : IRequestHandler<CreateUserFromAuthModuleCommand, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(CreateUserFromAuthModuleCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (existingUser.HasValue())
        {
            logger.LogWarning("Sign - Object {ObjectName} with username: {UserName} already exists. Nothing to create from auth module.", 
                nameof(User), request.UserName);
            return UserErrors.ValidationUserNameAlreadyExists;
        }
        
        var user = new User(request.UserName, request.FullName, true, request.Roles, request.Permissions);
        var userCreatedEvent = new UserCreatedFromAuthModuleEvent(user.Id, user.UserName, request.FullName, user.Roles, user.Permissions);
        unitOfWork
            .StartStream<UserCreatedFromAuthModuleEvent, User>(
                user.Id, userCreatedEvent);
        
        return user;
    }
}