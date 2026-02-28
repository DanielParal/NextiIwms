using ErrorOr;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.DetachBatch;

internal record DetachBatchCommand(Guid BatchId) : IWashingCommand<ErrorOr<Success>>;