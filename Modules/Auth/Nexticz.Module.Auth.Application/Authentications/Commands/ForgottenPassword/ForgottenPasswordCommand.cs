using ErrorOr;
using MediatR;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.ForgottenPassword;

public record ForgottenPasswordCommand(string Email) : IRequest<ErrorOr<Success>>;