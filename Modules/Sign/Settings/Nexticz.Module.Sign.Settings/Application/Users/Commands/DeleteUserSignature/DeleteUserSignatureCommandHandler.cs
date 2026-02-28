using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.FileHandling;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.DeleteUserSignature;

internal class DeleteUserSignatureCommandHandler(ISender sender,
    ILogger<DeleteUserSignatureCommandHandler> logger,
    ISettingsFileHandler settingsFileHandler,
    ISettingsUnitOfWork unitOfWork) : IRequestHandler<DeleteUserSignatureCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteUserSignatureCommand request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning("Sign - Did not find object {ObjectName} with UserName: {UserName}. Cannot delete user signature.", 
                nameof(User), request.UserName);
            return UserErrors.ValidationUserNameDoesNotExist;
        }

        settingsFileHandler.DeleteUserSignature(user.Value.Id);
        
        var userSignatureDeletedEvent = new UserSignatureDeletedEvent(user.Value.Id, user.Value.UserName);
        unitOfWork.AppendEvent(user.Value.Id, userSignatureDeletedEvent);
        
        logger.LogInformation("Sign - User signature for user with username: {UserName} deleted.", 
            request.UserName);
        
        return Result.Success;
    }
}