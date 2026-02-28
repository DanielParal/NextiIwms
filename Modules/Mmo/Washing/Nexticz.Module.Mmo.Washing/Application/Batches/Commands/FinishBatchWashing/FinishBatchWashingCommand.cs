using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishBatchWashing;

internal record FinishBatchWashingCommand(Guid BatchId, DateTimeOffset DateFinished) : IWashingCommand<ErrorOr<Success>>;