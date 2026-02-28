using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.RefreshTokens.Queries.GetRefreshTokensByUseId;

public class
    GetRefreshTokensByUserIdQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    : IRequestHandler<GetRefreshTokensByUserIdQuery,
        ErrorOr<List<AppUserRefreshToken>>>
{
    public async Task<ErrorOr<List<AppUserRefreshToken>>> Handle(GetRefreshTokensByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var appUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == query.UserId, cancellationToken);

        if (appUser == null)
            return AppUserErrors.AppUserWithIdDoesNotExist;

        return await unitOfWork.AppUserRefreshTokensRepository.GetAppUserRefreshTokensByUserIdAsync(query.UserId,
            cancellationToken);
    }
}