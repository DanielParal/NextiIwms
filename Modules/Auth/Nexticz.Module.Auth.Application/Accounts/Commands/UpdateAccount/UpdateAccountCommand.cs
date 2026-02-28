using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Auth.Application.Accounts.Commands.UpdateAccount;

public record UpdateAccountCommand(Guid Id, UpdateAccountRequest UpdateAccountRequest)
    : IRequest<ErrorOr<Updated>>;