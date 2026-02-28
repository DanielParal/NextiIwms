using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;

namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.CreateLoadingActionsNda;

public class CreateLoadingActionsNdaCommand : IRequest<ErrorOr<Created>>
{
    public required CreateLoadingActionsNdaRequest CreateLoadingActionsNdaRequest { get; set; }
}