using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Auth.Application.Accounts.Queries.GetAccountByUsername;

public record GetAccountByUsernameQuery(string Username) : IRequest<ErrorOr<AccountResponse>>;