using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Application.Users.Queries.GetUserByEmail;
using Nexticz.Module.Auth.Application.Users.Queries.GetUserById;
using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Auth.Application.Users.Commands.UpdateUser;

internal record UpdateUserCommand(Guid Id, UpdateAccountRequest UpdateAccountRequest)
    : IRequest<ErrorOr<Updated>>;
    
internal class UpdateUserCommandHandler(
    ILogger<UpdateUserCommandHandler> logger,
    ISender sender,
    IUserWriteRepository userWriteRepository,
    IAuthPublisher authPublisher)
    : IRequestHandler<UpdateUserCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [UpdateUserCommandHandler] id: {Id}, email: {Email}", 
            command.Id, command.UpdateAccountRequest.Email);

        var originUser = await sender.Send(new GetUserByIdQuery(command.Id), cancellationToken);
        if (originUser.IsError)
        {
            logger.LogInformation("[Auth] [End] [UpdateUserCommandHandler] user with id: {Id} does not exist", 
                command.Id);
            return UserErrors.ValidationUserWithIdDoesNotExist;
        }

        if (!string.Equals(originUser.Value.Email, command.UpdateAccountRequest.Email, StringComparison.CurrentCultureIgnoreCase))
        {
            var existingUserWithTheSameEmail = await sender.Send(new GetUserByEmailQuery(command.UpdateAccountRequest.Email), cancellationToken);
            if (existingUserWithTheSameEmail.HasValue())
            {
                logger.LogInformation("[Auth] [End] [UpdateUserCommandHandler] user with email already exists: {Email}, id: {Id}", 
                    command.UpdateAccountRequest.Email, command.Id);
                return UserErrors.ValidationUserWithEmailAlreadyExists;
            }
        }
        
        var updateDomainResult = originUser.Value.Update(
            command.UpdateAccountRequest.Email,
            command.UpdateAccountRequest.PhoneNumber,
            command.UpdateAccountRequest.Firstname,
            command.UpdateAccountRequest.Lastname,
            command.UpdateAccountRequest.Company,
            command.UpdateAccountRequest.BlockedFrom,
            command.UpdateAccountRequest.Roles,
            command.UpdateAccountRequest.Permissions);

        if (updateDomainResult.IsError)
        {
            logger.LogInformation("[Auth] [End] [UpdateUserCommandHandler] error updating user. Id: {Id}, Email: {Email}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                command.Id, command.UpdateAccountRequest.Email, updateDomainResult.FirstError.Code, updateDomainResult.FirstError.Description);
            return updateDomainResult.Errors;
        }
        
        var updateResult = await userWriteRepository.UpdateUserAsync(originUser.Value, command.UpdateAccountRequest.Password, cancellationToken);
        
        if (updateResult.IsError)
        {
            logger.LogWarning("[Auth] [End] [UpdateUserCommandHandler] error updating user in the db. Id: {Id}, Email: {Email}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                command.Id, command.UpdateAccountRequest.Email, updateResult.FirstError.Code, updateResult.FirstError.Description);
            return updateResult.Errors;
        }
        
        logger.LogInformation("[Auth] [End] [UpdateUserCommandHandler] user updated, id: {Id}, email: {Email}", 
            originUser.Value.Id, originUser.Value.UserName);
        
        await authPublisher.PublishUserChangedMessageAsync(
            originUser.Value.Id,
            originUser.Value.UserName,
            originUser.Value.FirstName,
            originUser.Value.LastName,
            originUser.Value.Roles,
            originUser.Value.Permissions,
            UserChangeTypeContract.Updated,
            cancellationToken);
        
        return Result.Updated;
    }
}