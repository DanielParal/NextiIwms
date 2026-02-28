using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Auth.Application.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(CreateAccountRequest CreateAccountRequest) : IRequest<ErrorOr<AppUser>>;