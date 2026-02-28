using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.ActivityCategories.Queries.GetActivityCategoryById;

public class GetActivityCategoriesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetActivityCategoriesQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetActivityCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.ActivityCategoriesRepository.GetActivityCategoriesAsync(query.FilteringParams,
            cancellationToken);
    }
}