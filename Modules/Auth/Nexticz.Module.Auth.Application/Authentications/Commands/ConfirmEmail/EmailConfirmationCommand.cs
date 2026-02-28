using ErrorOr;
using MediatR;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.ConfirmEmail;

public class EmailConfirmationCommand : IRequest<ErrorOr<Success>>
{
    public required string Token { get; set; }
    public required string Email { get; set; }
}