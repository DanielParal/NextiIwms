using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Users;
using Nexticz.Module.Mmo.Settings.Contracts.Users.Queries;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUsersByNotification;

internal class GetUsersByNotificationQueryHandler(IUserReadOnlyRepository userReadOnlyRepository) 
    : IRequestHandler<GetUsersByNotificationQuery, UserResponse[]>
{
    public async Task<UserResponse[]> Handle(GetUsersByNotificationQuery request, CancellationToken cancellationToken)
    {
        var users = await userReadOnlyRepository.GetUsersByReceivableNotificationAsync(
            (ReceivableNotification)request.ReceivableNotification,
            cancellationToken);
        return users.Select(UserResponseFactory.Create).ToArray();
    }
}