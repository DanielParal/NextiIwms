using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Contracts.Authentications;

namespace Nexticz.Module.Auth.Application.Authentications.Queries.GetAccountInfo;

public record GetAccountInfoQuery : IRequest<ErrorOr<GetAccountInfoResponse>>
{
    public required string Username { get; set; }
};