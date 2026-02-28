using ErrorOr;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.FinishKit;

internal record FinishKitCommand(Guid KitId) : IDryingCommand<ErrorOr<Success>>;