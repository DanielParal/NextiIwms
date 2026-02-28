using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUsers;

internal record GetUsersQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<User>>;