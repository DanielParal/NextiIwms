using ErrorOr;
using MediatR;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string Password) : IRequest<ErrorOr<Success>>;