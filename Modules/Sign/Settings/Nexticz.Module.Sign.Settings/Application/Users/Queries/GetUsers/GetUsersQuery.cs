using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsers;

internal record GetUsersQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<User>>;