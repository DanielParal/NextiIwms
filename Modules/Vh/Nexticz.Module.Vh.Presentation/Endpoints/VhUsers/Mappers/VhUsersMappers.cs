using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Module.Vh.Domain.VhUsers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.VhUsers.Mappers;

public static class VhUsersMappers
{
    public static VhUserResponse MapToVhUserResponse(this VhUser vhUser)
    {
        return new VhUserResponse
        {
            Id = vhUser.Id,
            Username = vhUser.Username,
            Centers = vhUser.Centers!.Select(x => x.Code).ToArray()
        };
    }
}