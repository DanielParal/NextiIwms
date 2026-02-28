using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.SplitBatch;

internal record SplitBatchCommand(Guid BatchId, string LineQueueCode, int CountToChange, int KitsCountToCreate) : IPlanningCommand<ErrorOr<Success>>;