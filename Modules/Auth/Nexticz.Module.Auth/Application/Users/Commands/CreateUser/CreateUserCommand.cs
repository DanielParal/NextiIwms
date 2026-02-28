using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Application.Users.Queries.GetUserByEmail;
using Nexticz.Module.Auth.Application.Users.Queries.GetUserByUsername;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Commands.CreateUser;

internal record CreateUserCommand(CreateAccountRequest CreateAccountRequest) : IRequest<ErrorOr<User>>;

internal class CreateUserCommandHandler(
    ISender sender,
    IAuthPublisher authPublisher,
    ILogger<CreateUserCommandHandler> logger,
    IUserWriteRepository userWriteRepository)
    : IRequestHandler<CreateUserCommand, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [CreateUserCommandHandler] username: {UserName}, email: {Email}", command.CreateAccountRequest.Username, command.CreateAccountRequest.Email);
        
        var user = await ValidateRequestAsync(command, cancellationToken);
        if (user.IsError) 
            return user.Errors;
        
        var createResult = await userWriteRepository.CreateUserAsync(user.Value, command.CreateAccountRequest.Password, cancellationToken);

        if (createResult.IsError)
        {
            logger.LogWarning("[Auth] [End] [CreateUserCommandHandler] error creating user in the db. UserName: {UserName}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                command.CreateAccountRequest.Username, createResult.FirstError.Code, createResult.FirstError.Description);
            return createResult.Errors;
        }
        
        logger.LogInformation("[Auth] [End] [CreateUserCommandHandler] user created, id: {Id}, username: {username}", 
            user.Value.Id, user.Value.UserName);
        
        await authPublisher.PublishUserChangedMessageAsync(
            user.Value.Id,
            user.Value.UserName,
            user.Value.FirstName,
            user.Value.LastName,
            user.Value.Roles,
            user.Value.Permissions,
            UserChangeTypeContract.Created,
            cancellationToken);
        
        return user.Value;
    }

    private async Task<ErrorOr<User>> ValidateRequestAsync(CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var existingUser = await sender.Send(new GetUserByUsernameQuery(command.CreateAccountRequest.Username), cancellationToken);
        if (existingUser.HasValue())
        {
            logger.LogInformation("[Auth] [End] [CreateUserCommandHandler] username: {UserName} already exists", command.CreateAccountRequest.Username);
            return UserErrors.ValidationUserWithUserNameAlreadyExists;
        }

        existingUser = await sender.Send(new GetUserByEmailQuery(command.CreateAccountRequest.Email), cancellationToken);
        
        if (existingUser.HasValue())
        {
            logger.LogInformation("[Auth] [End] [CreateUserCommandHandler] email: {Email} already exists", command.CreateAccountRequest.Email);
            return UserErrors.ValidationUserWithEmailAlreadyExists;
        }

        var user = User.CreateFrom(
            command.CreateAccountRequest.Username,
            command.CreateAccountRequest.Email,
            command.CreateAccountRequest.PhoneNumber,
            command.CreateAccountRequest.Firstname,
            command.CreateAccountRequest.Lastname,
            command.CreateAccountRequest.Company,
            command.CreateAccountRequest.Roles,
            command.CreateAccountRequest.Permissions,
            [],
            command.CreateAccountRequest.BlockedFrom);

        if (user.IsError)
        {
            logger.LogInformation("[Auth] [End] [CreateUserCommandHandler] error creating user. UserName: {UserName}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                command.CreateAccountRequest.Username, user.FirstError.Code, user.FirstError.Description);
            return user.Errors;
        }
        
        if (user.Value.Roles.Length == 0)
            user.Value.AddDefaultRole();
        
        if (user.Value.Permissions.Length == 0)
            user.Value.AddDefaultPermission();
        
        return user;
    }
}