using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.ActivityCategories.Commands.DeleteActivityCategory;

public class DeleteActivityCategoryCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}