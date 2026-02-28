using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Workers.Common.Models;


namespace Nexticz.Module.Vh.Application.Workers.Queries.GetWorkers;

public class GetWorkersQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required WorkersFilteringParams FilteringParams { get; set; }
}