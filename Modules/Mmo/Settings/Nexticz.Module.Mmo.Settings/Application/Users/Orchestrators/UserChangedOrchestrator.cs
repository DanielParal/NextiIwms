using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel.Security;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Users.Commands.CreateUser;
using Nexticz.Module.Mmo.Settings.Application.Users.Commands.DeleteUser;
using Nexticz.Module.Mmo.Settings.Application.Users.Commands.UpdateUserFromAuthModule;
using Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserByUserName;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Orchestrators;

internal class UserChangedOrchestrator(
    ILogger<UserChangedOrchestrator> logger,
    ISender sender) : IUserChangedOrchestrator
{
    private static string[] MmoRoles { get; } = RoleHelper.GetNames();
    private static string[] MmoPermissions { get; } = PermissionHelper.GetNames();
    
    public async Task OrchestrateAsync(UserChangedMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation("MMO Settings - Account changed notification received. UserName: {UserName}. UserChangeType: {UserChangeType}, Roles: {Roles}, Permissions: {Permissions}.",
            message.Username, message.UserChangeType, message.Roles, message.Permissions);

        if (string.IsNullOrWhiteSpace(message.Username))
        {
            logger.LogWarning("MMO Settings - cannot handle notification. UserName is empty.");
            return;
        }
        
        var filteredRoles = message.Roles.Where(x => MmoRoles.Contains(x)).ToArray();
        var filteredPermissions = message.Permissions.Where(x => MmoPermissions.Contains(x)).ToArray();
        
        switch (message.UserChangeType)
        {
            case UserChangeTypeContract.Created:
                await CreateUserAsync(message.Username, filteredRoles, filteredPermissions, cancellationToken);
                break;
            case UserChangeTypeContract.Updated:
                await UpdateOrCreateUserAsync(message.Username, filteredRoles, filteredPermissions, cancellationToken);
                break;
            case UserChangeTypeContract.Deleted:
                await DeleteUserAsync(message.Username, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private async Task CreateUserAsync(string userName, string[] filteredRoles, string[] filteredPermissions, CancellationToken cancellationToken)
    {
        var isMmoUser = filteredRoles.Length != 0;

        if (!isMmoUser)
        {
            logger.LogInformation("MMO Settings - Account is not for MMO module. UserName: {UserName}. UserChangeType: {UserChangeType}, Roles: {Roles}, Permissions: {Permissions}.",
                userName, UserChangeTypeContract.Created, filteredRoles, filteredPermissions);
            return;
        }
        
        var result = await sender.Send(new CreateUserCommand(userName, filteredRoles, filteredPermissions), cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("MMO Settings - Error creating user: {UserName} in MMO Settings. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                userName, result.FirstError.Code, result.FirstError.Description);
            return;
        }
        
        logger.LogInformation("MMO Settings - User {UserName} created in MMO Settings.", userName);
    }
    
    private async Task UpdateOrCreateUserAsync(string userName, string[] filteredRoles, string[] filteredPermissions, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(userName), cancellationToken);
        if (!user.HasValue())
        {
            await CreateUserAsync(userName, filteredRoles, filteredPermissions, cancellationToken);
            return;
        }
        
        var result = await sender.Send(new UpdateUserFromAuthModuleCommand(userName, filteredRoles, filteredPermissions), cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("MMO Settings - Error updating user: {UserName} in MMO Settings. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                userName, result.FirstError.Code, result.FirstError.Description);
            return;
        }
        
        logger.LogInformation("MMO Settings - User {UserName} updated in MMO Settings.", userName);
    }
    
    private async Task DeleteUserAsync(string userName, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(userName), cancellationToken);
        if (!user.HasValue())
        {
            logger.LogInformation("MMO Settings - Account is not for MMO module. UserName: {UserName}. UserChangeType: {UserChangeType}.",
                userName, UserChangeTypeContract.Deleted);
            return;
        }
        
        var result = await sender.Send(new DeleteUserCommand(userName), cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("MMO Settings - Error deleting user: {UserName} in MMO Settings. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                userName, result.FirstError.Code, result.FirstError.Description);
            return;
        }
        
        logger.LogInformation("MMO Settings - User {UserName} deleted in MMO Settings.", userName);
    }
}