using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUserById;

internal class GetUserByIdQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetUserByIdQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await readOnlyEventStoreRepository.GetByIdAsync<User>(request.Id, cancellationToken);

        if (user is null)
            return UserErrors.UserNotFound;

        return user;
    }
}