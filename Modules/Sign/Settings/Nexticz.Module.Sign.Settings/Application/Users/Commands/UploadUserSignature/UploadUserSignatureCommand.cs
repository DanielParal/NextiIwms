using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.UploadUserSignature;

internal record UploadUserSignatureCommand(
    string UserName, 
    IFormFile FormFile) : ISettingsCommand<ErrorOr<Success>>;