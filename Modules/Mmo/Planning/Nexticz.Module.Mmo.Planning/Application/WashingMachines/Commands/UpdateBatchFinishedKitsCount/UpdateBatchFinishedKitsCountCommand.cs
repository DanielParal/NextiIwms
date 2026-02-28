using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.UpdateBatchFinishedKitsCount;

internal record UpdateBatchFinishedKitsCountCommand(
    Guid BatchId, string LineQueueCode, DateTimeOffset DateFinished) : IPlanningCommand<ErrorOr<Batch>>;