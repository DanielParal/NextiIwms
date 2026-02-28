using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Authentications;

namespace Nexticz.Module.Auth.Application.Authentications.Queries.GetLoggedUserInfo;

public class GetLoggedUserInfoQuery : IRequest<ErrorOr<AuthenticationResponse>>
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}