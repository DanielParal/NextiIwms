using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.RefreshTokens.Commands.DeleteRefreshTokenById;

public class DeleteRefreshTokenByIdCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteRefreshTokenByIdCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteRefreshTokenByIdCommand command,
        CancellationToken cancellationToken)
    {
        var token = await unitOfWork.AppUserRefreshTokensRepository.GetAppUserRefreshTokenByIdAsync(command.Id,
            cancellationToken);

        if (token is null)
            return AppUserErrors.DeleteAppUserRefreshTokenError;
        
        unitOfWork.Remove(token);

        await unitOfWork.CompleteAsync(cancellationToken);
        
        return Result.Deleted;
    }
}