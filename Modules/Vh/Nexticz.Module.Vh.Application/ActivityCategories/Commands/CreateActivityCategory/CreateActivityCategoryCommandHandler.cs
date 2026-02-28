using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.ActivityCategories.Commands.CreateActivityCategory;

public class CreateActivityCategoryCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateActivityCategoryCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateActivityCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var activityType = Enum.TryParse(command.CreateActivityCategoryRequest.ActivityType.ToString(),
            out ActivityType domainActivityType);

        if (activityType is false) return ActivityCategoryErrors.CreateActivityCategoryError;

        var activityCategory = new ActivityCategory
        {
            Name = command.CreateActivityCategoryRequest.Name, Color = command.CreateActivityCategoryRequest.Color,
            ActivityType = domainActivityType
        };

        unitOfWork.Add(activityCategory);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}