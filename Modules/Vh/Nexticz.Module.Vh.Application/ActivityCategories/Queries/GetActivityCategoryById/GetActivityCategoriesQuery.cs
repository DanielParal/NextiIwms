using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.ActivityCategories.Common.Models;


namespace Nexticz.Module.Vh.Application.ActivityCategories.Queries.GetActivityCategoryById;

public class GetActivityCategoriesQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required ActivityCategoriesFilteringParams FilteringParams { get; set; }
}