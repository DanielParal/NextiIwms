using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.ActivityCategories;

namespace Nexticz.Module.Vh.Application.ActivityCategories.Commands.CreateActivityCategory;

public class CreateActivityCategoryCommand : IRequest<ErrorOr<Created>>
{
    public required CreateActivityCategoryRequest CreateActivityCategoryRequest { get; set; }
}