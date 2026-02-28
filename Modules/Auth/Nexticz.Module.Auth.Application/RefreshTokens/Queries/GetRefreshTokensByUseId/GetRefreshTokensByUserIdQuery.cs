using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;

namespace Nexticz.Module.Auth.Application.RefreshTokens.Queries.GetRefreshTokensByUseId;

public record GetRefreshTokensByUserIdQuery(Guid UserId) : IRequest<ErrorOr<List<AppUserRefreshToken>>>;