using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Models;


namespace Nexticz.Module.Vh.Application.WorkerShifts.Queries.GetWorkerShifts;

public class GetWorkerShiftsQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required WorkerShiftsFilteringParams FilteringParams { get; set; }
}