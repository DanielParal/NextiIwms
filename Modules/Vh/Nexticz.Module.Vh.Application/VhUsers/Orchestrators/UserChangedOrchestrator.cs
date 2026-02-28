using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Module.Vh.Domain.VhUsers;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.VhUsers.Orchestrators;

internal class UserChangedOrchestrator(
    IUnitOfWork unitOfWork, 
    UserManager<AppUser> userManager,
    ILogger<UserChangedOrchestrator> logger,
    ISender sender) : IUserChangedOrchestrator
{
    private VhUser? VhUser { get; set; }
    private AppUser? Account { get; set; }
    private UserChangedMessage? Message { get; set; }
    private string[] VhRoles { get; } = [nameof(VhRole.VhMember)];
    
    public async Task OrchestrateAsync(UserChangedMessage message, CancellationToken cancellationToken)
    {
        Message = message;

        VhUser =
            await unitOfWork
                .VhUsersRepository
                .GetVhUserByUsernameAsync(message.Username, cancellationToken);

        Account =
            await userManager
                .Users
                .FirstOrDefaultAsync(x => x.UserName == message.Username, cancellationToken);

        switch (message.UserChangeType)
        {
            case UserChangeTypeContract.Created:
                await CreateVhUser(cancellationToken);
                break;
            case UserChangeTypeContract.Updated:
                await UpdateVhUser(cancellationToken);
                break;
            case UserChangeTypeContract.Deleted:
                await DeleteVhUser(cancellationToken);
                break;
            default:
                throw new ArgumentException();
        }
    }
    
    private async Task DeleteVhUser(CancellationToken cancellationToken)
    {
        if (VhUser == null)
        {
            return;
        }
        
        unitOfWork.Remove(VhUser);

        logger.LogInformation("VH - VhUser with username {UserName} deleted.", Message!.Username);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    private async Task UpdateVhUser(CancellationToken cancellationToken)
    {
        if (VhUser == null && Account == null)
        {
            logger.LogWarning("VH - UpdateVhUser - VhUser with username {UserName} not exists.", Message!.Username);
            return;
        }

        if (VhUser == null)
        {
            await CreateVhUser(cancellationToken);
            return;
        }

        VhUser.Active = Message!.Roles.Any(x => VhRoles.Contains(x));

        logger.LogInformation("VH - UpdateVhUser - VhUser with username {UserName} updated.", Message!.Username);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    private async Task CreateVhUser(CancellationToken cancellationToken)
    {
        if (VhUser != null || Account == null)
        {
            logger.LogWarning("VH - VhUser with username {UserName} already exists.", Message!.Username);
            return;
        }

        if (!Message!.Roles.Any(x => VhRoles.Contains(x)))
            return;

        var newVhUser = new VhUser { Id = Account.Id, Active = true, Username = Message!.Username! };

        unitOfWork.Add(newVhUser);

        logger.LogInformation("VH - VhUser with username {UserName} created.", Message!.Username);
        await unitOfWork.CompleteAsync(cancellationToken);
    }
}