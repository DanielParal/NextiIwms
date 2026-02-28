using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Application.Users.Queries.GetUsers;

namespace Nexticz.Module.Auth.Application.Users.Orchestrators;

internal class SyncUserOrchestrator(
    ISender sender,
    IAuthPublisher authPublisher) : ISyncUserOrchestrator
{
    public async Task<ErrorOr<Success>> OrchestrateAsync(CancellationToken cancellationToken)
    {
        var result = 
            await sender.Send(new GetUsersQuery(new BaseFilteringParams()), cancellationToken);

        if (result.IsError)
            return result.Errors;

        foreach (var user in result.Value.Data)
        {
            await authPublisher.PublishUserChangedMessageAsync(
                user.Id, 
                user.UserName, 
                user.FirstName, 
                user.LastName, 
                user.Roles, 
                user.Permissions, 
                UserChangeTypeContract.Updated, 
                cancellationToken);
        }

        return Result.Success;
    }
}