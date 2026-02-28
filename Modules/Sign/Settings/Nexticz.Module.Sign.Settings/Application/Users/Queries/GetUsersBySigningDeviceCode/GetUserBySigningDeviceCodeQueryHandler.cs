using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersBySigningDeviceCode;

internal class GetUsersBySigningDeviceCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetUsersBySigningDeviceCodeQuery, User[]>
{
    public async Task<User[]> Handle(GetUsersBySigningDeviceCodeQuery request, CancellationToken cancellationToken)
    {
        var upperCode = request.SigningDeviceCode.ToUpperInvariant();
        var users = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<User>(
                x => x.SigningDeviceCodes.Any(y => y == upperCode), 
                cancellationToken);
        
        return users.ToArray();
    }
}