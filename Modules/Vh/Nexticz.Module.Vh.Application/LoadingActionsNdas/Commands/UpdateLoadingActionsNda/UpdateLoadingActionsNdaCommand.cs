using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;

namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.UpdateLoadingActionsNda;

public class UpdateLoadingActionsNdaCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateLoadingActionsNdaRequest UpdateLoadingActionsNdaRequest { get; set; }
}