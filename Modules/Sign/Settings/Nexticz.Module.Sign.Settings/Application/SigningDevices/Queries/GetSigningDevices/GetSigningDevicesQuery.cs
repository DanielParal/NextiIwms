using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevices;

internal record GetSigningDevicesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<SigningDevice>>;