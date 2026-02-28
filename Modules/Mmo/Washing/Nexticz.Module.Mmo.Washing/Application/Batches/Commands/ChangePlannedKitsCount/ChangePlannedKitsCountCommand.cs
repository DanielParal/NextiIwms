using ErrorOr;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.ChangePlannedKitsCount;

internal record ChangePlannedKitsCountCommand(Guid BatchId, int PlannedKitsCount) : IWashingCommand<ErrorOr<Success>>;