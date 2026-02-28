using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.ActivityCategories.Commands.UpdateActivityCategory;

public class UpdateActivityCategoryCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateActivityCategoryCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateActivityCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var activityCategory =
            await unitOfWork.ActivityCategoriesRepository.GetActivityCategoryByIdAsync(command.Id, cancellationToken);

        if (activityCategory is null)
            return ActivityCategoryErrors.ActivityCategoryWithIdDoesnotExist;

        var activityType = Enum.TryParse(command.UpdateActivityCategoryRequest.ActivityType.ToString(),
            out ActivityType domainActivityType);

        if (activityType is false) return ActivityCategoryErrors.UpdateActivityCategoryError;

        activityCategory.Name = command.UpdateActivityCategoryRequest.Name;
        activityCategory.Color = command.UpdateActivityCategoryRequest.Color;
        activityCategory.ActivityType = domainActivityType;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}