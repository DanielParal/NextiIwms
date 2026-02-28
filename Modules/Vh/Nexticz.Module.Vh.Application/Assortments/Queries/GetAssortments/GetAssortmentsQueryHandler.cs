using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.Assortments.Queries.GetAssortments;

public class GetAssortmentsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAssortmentsQuery, ErrorOr<FilteredResult>>
{
    public async  Task<ErrorOr<FilteredResult>> Handle(GetAssortmentsQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.AssortmentRepository.GetAssortmentsAsync(query.FilteringParams, cancellationToken);
    }
}