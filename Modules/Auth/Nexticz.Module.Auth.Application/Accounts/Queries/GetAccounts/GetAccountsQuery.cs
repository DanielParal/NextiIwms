using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Auth.Application.Accounts.Common.Models;


namespace Nexticz.Module.Auth.Application.Accounts.Queries.GetAccounts;

public record GetAccountsQuery(AccountsFilteringParams FilteringParams) : IRequest<ErrorOr<FilteredResult>>;