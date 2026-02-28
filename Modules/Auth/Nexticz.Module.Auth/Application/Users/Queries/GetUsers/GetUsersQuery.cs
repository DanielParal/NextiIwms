using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Queries.GetUsers;

public record GetUsersQuery(BaseFilteringParams FilteringParams) : IRequest<ErrorOr<FilteredResult<User>>>;