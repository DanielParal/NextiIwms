using ErrorOr;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Commands.CallSos;

internal record CallSosCommand(string WashingMachineCode) : IWashingCommand<ErrorOr<Success>>;