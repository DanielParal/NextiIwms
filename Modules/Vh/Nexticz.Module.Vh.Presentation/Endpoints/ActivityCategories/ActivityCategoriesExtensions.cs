using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.ActivityCategories;

public static class ActivityCategoriesExtensions
{
    public static IEndpointRouteBuilder MapActivityCategoriesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateActivityCategory()
            .MapUpdateActivityCategory()
            .MapDeleteActivityCategory()
            .MapGetActivityCategoryById()
            .MapGetActivityCategories();
    }
}