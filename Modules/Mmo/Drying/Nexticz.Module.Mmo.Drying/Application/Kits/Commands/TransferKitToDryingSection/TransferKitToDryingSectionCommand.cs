using ErrorOr;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.TransferKitToDryingSection;

internal record TransferKitToDryingSectionCommand(Guid KitId) : IDryingCommand<ErrorOr<Success>>;