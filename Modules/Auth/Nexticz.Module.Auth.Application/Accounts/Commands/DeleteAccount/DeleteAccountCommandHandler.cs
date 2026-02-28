using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Auth.Application.Accounts.Queries.GetAccountById;
using Nexticz.Module.Auth.Application.MasstransitPublishers;

namespace Nexticz.Module.Auth.Application.Accounts.Commands.DeleteAccount;

public class DeleteAccountCommandHandler(
    UserManager<AppUser> userManager,
    IAuthPublisher authPublisher,
    ISender sender,
    ILogger<DeleteAccountCommandHandler> logger)
    : IRequestHandler<DeleteAccountCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var appUser = await userManager.Users.Where(x => x.Id == command.Id).FirstOrDefaultAsync(cancellationToken);

        if (appUser is null) return AppUserErrors.AppUserWithUsernameDoesNotExist;
        
        var appUserToPublish = await sender.Send(new GetAccountByIdQuery(appUser.Id), cancellationToken);
        
        await userManager.DeleteAsync(appUser);

        logger.LogInformation("AUTH - user deleted, id: {Id}, username: {username}", 
            appUserToPublish.Value.Id, appUserToPublish.Value.Username);
        
        await authPublisher.PublishUserChangedMessageAsync(
            appUserToPublish.Value.Id,
            appUserToPublish.Value.Username,
            appUserToPublish.Value.Firstname,
            appUserToPublish.Value.Lastname,
            appUserToPublish.Value.Roles,
            appUserToPublish.Value.Permissions,
            UserChangeTypeContract.Deleted,
            cancellationToken);

        return Result.Deleted;
    }
}