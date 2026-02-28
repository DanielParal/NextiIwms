using MediatR;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Auth.Contracts.Me;

namespace Nexticz.Module.Auth.Application.Me.Queires.GetMe;

internal record GetMeQuery : IRequest<MeResponse>;

internal class GetMeQueryHandler(ICurrentUserProvider currentUserProvider) : IRequestHandler<GetMeQuery, MeResponse>
{
    public Task<MeResponse> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var meResponse = new MeResponse(currentUser.Id, currentUser.UserName, currentUser.Roles.ToArray(), currentUser.Permissions.ToArray());
        return Task.FromResult(meResponse);
    }
}