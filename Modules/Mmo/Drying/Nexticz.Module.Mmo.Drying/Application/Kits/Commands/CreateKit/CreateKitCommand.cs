using ErrorOr;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.CreateKit;

internal record CreateKitCommand(Guid KitId, int GlobalKitsCount, Guid BatchId, string LineCode, string KitCode, DateTimeOffset DateFinished) : IDryingCommand<ErrorOr<Success>>;