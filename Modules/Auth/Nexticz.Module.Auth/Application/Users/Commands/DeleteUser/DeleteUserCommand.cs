using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Application.Users.Queries.GetUserById;
using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Auth.Application.Users.Commands.DeleteUser;

internal record DeleteUserCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;

internal class DeleteUserCommandHandler(
    ISender sender,
    ILogger<DeleteUserCommandHandler> logger,
    IAuthPublisher authPublisher,
    IUserWriteRepository userWriteRepository)
    : IRequestHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [DeleteUserCommandHandler] id: {Id}", command.Id);        
        
        var user = await sender.Send(new GetUserByIdQuery(command.Id), cancellationToken);

        if (user.IsError)
        {
            logger.LogInformation("[Auth] [End] [DeleteUserCommandHandler] user already does not exist in the db. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                user.FirstError.Code, user.FirstError.Description);
            return Result.Deleted;
        }
        
        var result = await userWriteRepository.DeleteUserAsync(command.Id, cancellationToken);

        if (result.IsError)
        {
            logger.LogError("[Auth] [End] [DeleteUserCommandHandler] error deleting user from the db. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                result.FirstError.Code, result.FirstError.Description);
            return result.Errors;
        }
        
        logger.LogInformation("[Auth] [End] [DeleteUserCommandHandler] user deleted, id: {Id}, username: {UserName}", 
            user.Value.Id, user.Value.UserName);
        
        await authPublisher.PublishUserChangedMessageAsync(
            user.Value.Id,
            user.Value.UserName,
            user.Value.FirstName,
            user.Value.LastName,
            user.Value.Roles,
            user.Value.Permissions,
            UserChangeTypeContract.Deleted,
            cancellationToken);
        
        return Result.Deleted;
    }
}