using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.VhUsers.Common.Models;


namespace Nexticz.Module.Vh.Application.VhUsers.Queries;

public record GetVhUsersQuery(VhUsersFilteringParams FilteringParams) : IRequest<ErrorOr<FilteredResult>>;