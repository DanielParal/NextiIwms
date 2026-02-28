using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.ActivityCategories.Commands.DeleteActivityCategory;

public class DeleteActivityCategoryCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteActivityCategoryCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteActivityCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var activityCategory =
            await unitOfWork.ActivityCategoriesRepository.GetActivityCategoryByIdAsync(command.Id, cancellationToken);

        if (activityCategory is null) return ActivityCategoryErrors.ActivityCategoryWithIdDoesnotExist;

        unitOfWork.Remove(activityCategory);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}