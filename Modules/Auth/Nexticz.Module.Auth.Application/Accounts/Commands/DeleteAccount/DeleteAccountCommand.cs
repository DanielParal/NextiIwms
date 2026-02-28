using ErrorOr;
using MediatR;

namespace Nexticz.Module.Auth.Application.Accounts.Commands.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;