using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Auth.Application.Accounts.Common.Models;
using Nexticz.Module.Auth.Application.Accounts.Queries.GetAccounts;
using Nexticz.Module.Auth.Application.MasstransitPublishers;

namespace Nexticz.Module.Auth.Application.Accounts.Orchestrators;

internal class SyncAccountOrchestrator(
    ISender sender,
    IAuthPublisher authPublisher) : ISyncAccountOrchestrator
{
    public async Task<ErrorOr<Success>> OrchestrateAsync(CancellationToken cancellationToken)
    {
        var result = 
            await sender.Send(new GetAccountsQuery(new AccountsFilteringParams()), cancellationToken);

        if (result.IsError)
            return result.Errors;
                    
        foreach (var appUserToPublish in result.Value.data.OfType<AccountResponse>())
        {
            await authPublisher.PublishUserChangedMessageAsync(
                appUserToPublish.Id,
                appUserToPublish.Username,
                appUserToPublish.Firstname,
                appUserToPublish.Lastname,
                appUserToPublish.Roles,
                appUserToPublish.Permissions,
                UserChangeTypeContract.Updated,
                cancellationToken);
        }
        
        return Result.Success;
    }
}