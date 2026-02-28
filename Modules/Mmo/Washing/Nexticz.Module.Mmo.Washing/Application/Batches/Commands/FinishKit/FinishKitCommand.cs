using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.KitWashCycleEntity;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishKit;

internal record FinishKitCommand(Guid BatchId) : IWashingCommand<ErrorOr<KitWashCycle>>;