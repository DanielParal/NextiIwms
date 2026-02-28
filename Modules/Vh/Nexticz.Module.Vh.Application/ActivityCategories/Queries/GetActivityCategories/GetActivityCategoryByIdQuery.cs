using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.ActivityCategories;

namespace Nexticz.Module.Vh.Application.ActivityCategories.Queries.GetActivityCategories;

public class GetActivityCategoryByIdQuery : IRequest<ErrorOr<ActivityCategoryResponse>>
{
    public required Guid Id { get; set; }
}