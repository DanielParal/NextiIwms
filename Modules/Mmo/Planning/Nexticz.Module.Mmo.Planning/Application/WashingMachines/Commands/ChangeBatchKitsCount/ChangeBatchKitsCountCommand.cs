using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.ChangeBatchKitsCount;

internal record ChangeBatchKitsCountCommand(string LineQueueCode, Guid BatchId, int CountToChange) : IPlanningCommand<ErrorOr<Updated>>;