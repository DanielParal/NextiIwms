using System.Security.Claims;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.UserAggregate;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Repositories;

internal class UserWriteRepository(
    ILogger<UserWriteRepository> logger,
    RoleManager<AppRole> roleManager,
    IPasswordHasher<AppUser> passwordHasher,
    IPasswordValidator<AppUser> passwordValidator,
    UserManager<AppUser> userManager) : IUserWriteRepository
{
    public async Task<ErrorOr<Success>> CreateUserAsync(User user, string? password, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [CreateUserAsync] username: {UserName}, email: {Email}", user.UserName, user.Email);
        
        var newAppUser = new AppUser
        (
            user.UserName,
            user.Email,
            user.PhoneNumber,
            user.FirstName,
            user.LastName,
            user.Company,
            user.BlockedFrom,
            id: user.Id
        );
        
        var createdUserResult = string.IsNullOrWhiteSpace(password)
            ? await userManager.CreateAsync(newAppUser)
            : await userManager.CreateAsync(newAppUser, password);

        if (!createdUserResult.Succeeded)
        {
            logger.LogInformation("[Auth] [End] [CreateUserAsync] username: {UserName}, email: {Email}, errors: {@Errors}", user.UserName, user.Email, createdUserResult.Errors);
            return IdentityPasswordValidationMapper.CreateErrors(createdUserResult.Errors);
        }

        await CreateRolesIfNotExist(user.Roles);
        await userManager.AddToRolesAsync(newAppUser, user.Roles);
        
        var newUserClaims = user.Permissions
            .Select(x => new Claim(StringHelper.Claim.Type.MagicPermissions, x));
        
        await userManager.AddClaimsAsync(newAppUser, newUserClaims);
        
        logger.LogInformation("[Auth] [End] [CreateUserAsync] user created. Id: {Id}, username: {UserName}, email: {Email}", user.Id, user.UserName, user.Email);
        
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> UpdateUserAsync(User user, string? password, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [UpdateUserAsync] username: {UserName}, email: {Email}", user.UserName, user.Email);

        var appUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken);
        if (appUser is null)
        {
            logger.LogError("[Auth] [Error] [UpdateUserAsync] error updating user from the db. Usermanager didn't find user. Username: {UserName}, email: {Email}", 
                user.UserName, user.Email);
            return IdentityResultErrors.ErrorUpdatingUser;
        }

        if (!string.IsNullOrWhiteSpace(password))
        {
            var passwordValidationError = await ValidateAndHashPasswordAsync(appUser, password);
            if (!passwordValidationError.Success)
            {
                logger.LogError("[Auth] [Error] [UpdateUserAsync] error updating user. Invalid password. Id: {Id}, Username: {UserName}, Errors: {@Errors}", 
                    user.Id, user.UserName, passwordValidationError.Errors);
                return IdentityPasswordValidationMapper.CreateErrors(passwordValidationError.Errors);
            }
                
            appUser.PasswordHash = passwordValidationError.PasswordHash;
        }
        
        appUser.Email = user.Email;
        appUser.PhoneNumber = user.PhoneNumber;
        appUser.Firstname = user.FirstName;
        appUser.Lastname = user.LastName;
        appUser.Company = user.Company;
        appUser.BlockedFrom = user.BlockedFrom;
        
        await CreateRolesIfNotExist(user.Roles);
        var userInRoles = await userManager.GetRolesAsync(appUser);
        await userManager.RemoveFromRolesAsync(appUser, userInRoles);

        if (user.Roles.Length > 0)
        {
            await userManager.AddToRolesAsync(appUser, user.Roles);
        }
        
        var userClaims = await userManager.GetClaimsAsync(appUser);
        await userManager.RemoveClaimsAsync(appUser, userClaims);

        if (user.Permissions.Length > 0)
        {
            var newUserClaims = user.Permissions.Select(x => new Claim(StringHelper.Claim.Type.MagicPermissions, x));
            await userManager.AddClaimsAsync(appUser, newUserClaims);
        }
        
        var updateResult = await userManager.UpdateAsync(appUser);
        
        if (!updateResult.Succeeded)
        {
            logger.LogError("[Auth] [Error] [UpdateUserAsync] error updating user from the db. Id: {Id}, Errors: {@Errors}", user.Id, updateResult.Errors);
            return IdentityResultErrors.ErrorUpdatingUser;
        }
        
        logger.LogInformation("[Auth] [End] [UpdateUserAsync] user updated. Id: {Id}", user.Id);
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [DeleteUserAsync] id: {Id}", id);
        
        var appUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (appUser is null)
            return Result.Success;
        
        var result = await userManager.DeleteAsync(appUser);

        if (!result.Succeeded)
        {
            logger.LogError("[Auth] [Error] [DeleteUserAsync] error deleting user from the db. Errors: {@Errors}", result.Errors);
            return IdentityResultErrors.ErrorDeletingUser;
        }
        
        logger.LogInformation("[Auth] [End] [DeleteUserAsync] user deleted. Id: {Id}", id);
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> UpdateUserPasswordAsync(string username, string password, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [UpdateUserPasswordAsync] username: {UserName}", username);

        var appUser = await userManager.FindByNameAsync(username);
        if (appUser is null)
        {
            logger.LogError("[Auth] [Error] [UpdateUserPasswordAsync] error updating user's password from the db. Usermanager didn't find user. Username: {UserName}", 
                username);
            return IdentityResultErrors.ErrorUpdatingUser;
        }

        var passwordValidationError = await ValidateAndHashPasswordAsync(appUser, password);
        if (!passwordValidationError.Success)
        {
            logger.LogError("[Auth] [Error] [UpdateUserPasswordAsync] error updating user's password. Invalid password. Id: {Id}, Username: {UserName}, Errors: {@Errors}", 
                appUser.Id, appUser.UserName, passwordValidationError.Errors);
            return IdentityPasswordValidationMapper.CreateErrors(passwordValidationError.Errors);
        }
                
        appUser.PasswordHash = passwordValidationError.PasswordHash;
        
        await userManager.UpdateAsync(appUser);
        
        logger.LogInformation("[Auth] [End] [UpdateUserPasswordAsync] username: {UserName}", username);
        return Result.Success;
    }

    private async Task CreateRolesIfNotExist(IEnumerable<string> roles)
    {
        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
        
            if (!roleExist) 
                await roleManager.CreateAsync(new AppRole { Name = role });
        }
    }
    
    private async Task<(bool Success, string? PasswordHash, IEnumerable<IdentityError> Errors)> 
        ValidateAndHashPasswordAsync(AppUser user, string password)
    {
        var validation = await passwordValidator.ValidateAsync(userManager, user, password);
        if (!validation.Succeeded)
        {
            return (false, null, validation.Errors);
        }

        var hash = passwordHasher.HashPassword(user, password);
        return (true, hash, []);
    }

}