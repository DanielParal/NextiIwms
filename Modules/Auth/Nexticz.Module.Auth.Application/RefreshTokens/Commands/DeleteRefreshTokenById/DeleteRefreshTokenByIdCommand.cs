using ErrorOr;
using MediatR;

namespace Nexticz.Module.Auth.Application.RefreshTokens.Commands.DeleteRefreshTokenById;

public record DeleteRefreshTokenByIdCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;