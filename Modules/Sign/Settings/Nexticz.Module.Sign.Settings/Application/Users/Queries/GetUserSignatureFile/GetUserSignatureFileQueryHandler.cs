using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Sign.Settings.Application.FileHandling;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserSignatureFile;

internal class GetUserSignatureFileQueryHandler(
    ISender sender,
    ISettingsFileHandler settingsFileHandler,
    ILogger<GetUserSignatureFileQueryHandler> logger) : IRequestHandler<GetUserSignatureFileQuery, ErrorOr<FileResult>>
{
    public async Task<ErrorOr<FileResult>> Handle(GetUserSignatureFileQuery request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);
        if (user.IsError)
        {
            logger.LogWarning("Sign - Did not find object {ObjectName} with UserName: {UserName}. Cannot get user signature file.", 
                nameof(User), request.UserName);
            return UserErrors.ValidationUserNameDoesNotExist;
        }
        
        var fileResult = await settingsFileHandler.GetUserSignatureAsync(user.Value.Id, cancellationToken);

        if (fileResult is null)
        {
            logger.LogWarning("Sign - User signature not found. UserId: {UserId}, UserName: {UserName}", 
                user.Value.Id, request.UserName);
            return UserErrors.NotFoundUserSignatureFile;
        }
        
        return new FileResult(
            fileResult.ContentBytes, fileResult.ContentType, fileResult.FileName);
    }
}