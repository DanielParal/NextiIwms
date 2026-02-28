namespace Nexticz.Module.Auth.Application.Interfaces;

internal interface IPasswordValidator
{
    Task<bool> IsPasswordValidAsync(string username, string password, CancellationToken cancellationToken);
}