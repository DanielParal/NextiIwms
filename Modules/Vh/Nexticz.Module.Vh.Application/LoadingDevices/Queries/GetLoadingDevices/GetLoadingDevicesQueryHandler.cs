using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.LoadingDevices.Queries.GetLoadingDevices;

public class GetLoadingDevicesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLoadingDevicesQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetLoadingDevicesQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.LoadingDevicesRepository.GetLoadingDevicesAsync(query.FilteringParams,
            cancellationToken);
    }
}