using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.DeleteLoadingActionsNda;

public class DeleteLoadingActionsNdaCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}