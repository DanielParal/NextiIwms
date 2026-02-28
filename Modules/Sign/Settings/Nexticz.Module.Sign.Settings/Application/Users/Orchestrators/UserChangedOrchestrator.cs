using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel.Security;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Users.Commands.CreateUserFromAuthModule;
using Nexticz.Module.Sign.Settings.Application.Users.Commands.DeleteUserFromAuthModule;
using Nexticz.Module.Sign.Settings.Application.Users.Commands.UpdateUserFromAuthModule;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;

namespace Nexticz.Module.Sign.Settings.Application.Users.Orchestrators;

internal class UserChangedOrchestrator(
    ILogger<UserChangedOrchestrator> logger,
    ISender sender) : IUserChangedOrchestrator
{
    private static string[] MmoRoles { get; } = RoleHelper.GetNames();
    private static string[] MmoPermissions { get; } = PermissionHelper.GetNames();
    
    public async Task OrchestrateAsync(UserChangedMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation("SIGN - Settings - Account changed notification received. UserName: {UserName}. UserChangeType: {UserChangeType}, Roles: {Roles}, Permissions: {Permissions}.",
            message.Username, message.UserChangeType, message.Roles, message.Permissions);

        if (string.IsNullOrWhiteSpace(message.Username))
        {
            logger.LogWarning("SIGN - Settings - cannot handle notification. UserName is empty.");
            return;
        }
        
        var filteredRoles = message.Roles.Where(x => MmoRoles.Contains(x)).ToArray();
        var filteredPermissions = message.Permissions.Where(x => MmoPermissions.Contains(x)).ToArray();
        
        switch (message.UserChangeType)
        {
            case UserChangeTypeContract.Created:
                await CreateUserAsync(message.Username, message.FullName, filteredRoles, filteredPermissions, cancellationToken);
                break;
            case UserChangeTypeContract.Updated:
                await UpdateOrCreateUserAsync(message.Username, message.FullName, filteredRoles, filteredPermissions, cancellationToken);
                break;
            case UserChangeTypeContract.Deleted:
                await DeleteUserAsync(message.Username, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private async Task CreateUserAsync(string userName, string? fullName, string[] filteredRoles, string[] filteredPermissions, CancellationToken cancellationToken)
    {
        var isMmoUser = filteredRoles.Length != 0;

        if (!isMmoUser)
        {
            logger.LogInformation("SIGN - Settings - Account is not for SIGN module. UserName: {UserName}. UserChangeType: {UserChangeType}, Roles: {Roles}, Permissions: {Permissions}.",
                userName, UserChangeTypeContract.Created, filteredRoles, filteredPermissions);
            return;
        }
        
        var result = await sender.Send(new CreateUserFromAuthModuleCommand(
            userName, fullName, filteredRoles, filteredPermissions), cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("SIGN - Settings - Error creating user: {UserName} in SIGN Settings. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                userName, result.FirstError.Code, result.FirstError.Description);
            return;
        }
        
        logger.LogInformation("SIGN - Settings - User {UserName} created in SIGN Settings.", userName);
    }
    
    private async Task UpdateOrCreateUserAsync(string userName, string? fullName, string[] filteredRoles, string[] filteredPermissions, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(userName), cancellationToken);
        if (!user.HasValue())
        {
            await CreateUserAsync(userName, fullName, filteredRoles, filteredPermissions, cancellationToken);
            return;
        }
        
        var result = await sender.Send(new UpdateUserFromAuthModuleCommand(userName, fullName, filteredRoles, filteredPermissions), cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("SIGN - Settings - Error updating user: {UserName} in SIGN Settings. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                userName, result.FirstError.Code, result.FirstError.Description);
            return;
        }
        
        logger.LogInformation("SIGN - Settings - User {UserName} updated in SIGN Settings.", userName);
    }
    
    private async Task DeleteUserAsync(string userName, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(userName), cancellationToken);
        if (!user.HasValue())
        {
            logger.LogInformation("SIGN Settings - Account is not for SIGN module. UserName: {UserName}. UserChangeType: {UserChangeType}.",
                userName, UserChangeTypeContract.Deleted);
            return;
        }
        
        var result = await sender.Send(new DeleteUserFromAuthModuleCommand(userName), cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("SIGN Settings - Error deleting user: {UserName} in SIGN Settings. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                userName, result.FirstError.Code, result.FirstError.Description);
            return;
        }
        
        logger.LogInformation("SIGN Settings - User {UserName} deleted in SIGN Settings.", userName);
    }
}