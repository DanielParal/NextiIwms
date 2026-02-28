using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.ActivityCategories;

namespace Nexticz.Module.Vh.Application.ActivityCategories.Commands.UpdateActivityCategory;

public class UpdateActivityCategoryCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateActivityCategoryRequest UpdateActivityCategoryRequest { get; set; }
}