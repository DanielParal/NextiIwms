using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.ActivityCategories.Queries.GetActivityCategories;

public class GetActivityCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetActivityCategoryByIdQuery, ErrorOr<ActivityCategoryResponse>>
{
    public async Task<ErrorOr<ActivityCategoryResponse>> Handle(GetActivityCategoryByIdQuery query,
        CancellationToken cancellationToken)
    {
        var activityCategory =
            await unitOfWork.ActivityCategoriesRepository.GetActivityCategoryResponseByIdAsync(query.Id,
                cancellationToken);

        if (activityCategory is null) return ActivityCategoryErrors.ActivityCategoryWithIdDoesnotExist;

        return activityCategory;
    }
}