using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Centers.Queries.GetCenterById;

public class GetCenterByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCenterByIdQuery, ErrorOr<CenterResponse>>
{
    public async Task<ErrorOr<CenterResponse>> Handle(GetCenterByIdQuery query, CancellationToken cancellationToken)
    {
        var center = await unitOfWork.CentersRepository.GetCenterResponseByIdAsync(query.Id, cancellationToken);

        if (center is null)
        {
            return CenterErrors.CenterWithIdDoesnotExist;
        }

        return center;
    }
}