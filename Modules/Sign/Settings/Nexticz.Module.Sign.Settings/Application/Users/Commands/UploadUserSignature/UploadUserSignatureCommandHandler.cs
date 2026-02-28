using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.FileHandling;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.UploadUserSignature;

internal class UploadUserSignatureCommandHandler(
    ISender sender,
    ILogger<UploadUserSignatureCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISettingsFileHandler settingsFileHandler)
    : IRequestHandler<UploadUserSignatureCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UploadUserSignatureCommand request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (!user.HasValue())
        {
            logger.LogWarning("Sign - Object {ObjectName} with id: {UserName} does not exist. Nothing to update.", 
                nameof(User), request.UserName);
            return UserErrors.ValidationUserNameDoesNotExist;
        }
        
        var isFileValid = settingsFileHandler.IsFileValid(request.FormFile, maxFileSizeBytes: 250 * 1024, allowedExtensions: [".png", ".jpg", ".jpeg"]);
        if (isFileValid.IsError)
            return isFileValid.Errors;
        
        await settingsFileHandler.UploadUserSignatureAsync(request.FormFile, user.Value.Id, cancellationToken);
        
        var userSignatureUploadedEvent = 
            new UserSignatureUploadedEvent(user.Value.Id, user.Value.UserName);
        unitOfWork.AppendEvent(user.Value.Id, userSignatureUploadedEvent);

        logger.LogInformation("Sign - User signature for user with username: {UserName} uploaded.", 
            user.Value.UserName);
        
        return Result.Success;
    }
}