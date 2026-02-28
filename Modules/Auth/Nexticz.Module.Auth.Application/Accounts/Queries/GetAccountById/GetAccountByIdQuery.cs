using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Accounts;

namespace Nexticz.Module.Auth.Application.Accounts.Queries.GetAccountById;

public record GetAccountByIdQuery(Guid Id) : IRequest<ErrorOr<AccountResponse>>;