using ErrorOr;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.CreatePrinting;

internal record CreatePrintingCommand(string LineCode, Guid KitId) : IWashingCommand<ErrorOr<CreatePrintingCommandResponse>>;