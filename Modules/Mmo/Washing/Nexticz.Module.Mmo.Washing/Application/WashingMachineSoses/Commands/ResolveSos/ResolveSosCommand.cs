using ErrorOr;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Commands.ResolveSos;

internal record ResolveSosCommand(string WashingMachineCode) : IWashingCommand<ErrorOr<Success>>;