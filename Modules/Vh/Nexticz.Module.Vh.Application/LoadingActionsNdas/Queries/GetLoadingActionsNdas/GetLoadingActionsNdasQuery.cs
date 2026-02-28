using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Common.Models;


namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Queries.GetLoadingActionsNdas;

public class GetLoadingActionsNdasQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required LoadingActionsNdasFilteringParams FilteringParams { get; set; }
}