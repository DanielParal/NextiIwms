using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.LoadingDevices.Common.Models;


namespace Nexticz.Module.Vh.Application.LoadingDevices.Queries.GetLoadingDevices;

public class GetLoadingDevicesQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required LoadingDevicesFilteringParams FilteringParams { get; set; }
}