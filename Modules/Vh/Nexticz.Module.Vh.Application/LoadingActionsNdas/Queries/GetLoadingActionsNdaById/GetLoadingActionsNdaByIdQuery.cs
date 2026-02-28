using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;

namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Queries.GetLoadingActionsNdaById;

public class GetLoadingActionsNdaByIdQuery : IRequest<ErrorOr<LoadingActionsNdaResponse>>
{
    public required Guid Id { get; set; }
}