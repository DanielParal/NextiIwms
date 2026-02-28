using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkers;

internal record GetWorkersQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Worker>>;