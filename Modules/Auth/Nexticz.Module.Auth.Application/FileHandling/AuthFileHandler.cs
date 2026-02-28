using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.FileHandling;

namespace Nexticz.Module.Auth.Application.FileHandling;

internal class AuthFileHandler(
    ILogger<AuthFileHandler> logger) : FileHandler(logger), IAuthFileHandler
{
    
}