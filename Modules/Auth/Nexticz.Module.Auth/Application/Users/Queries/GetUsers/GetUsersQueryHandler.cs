using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserReadOnlyRepository userReadOnlyRepository)
    : IRequestHandler<GetUsersQuery, ErrorOr<FilteredResult<User>>>
{
    public async Task<ErrorOr<FilteredResult<User>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        return await userReadOnlyRepository.GetUsersAsync(query.FilteringParams, cancellationToken);
    }
}