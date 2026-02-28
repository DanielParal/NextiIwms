using ErrorOr;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.ConfirmSpecialInformation;

internal record ConfirmSpecialInformationCommand(Guid BatchId) : IWashingCommand<ErrorOr<Success>>;