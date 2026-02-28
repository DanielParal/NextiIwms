using ErrorOr;

namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Commands.LeaveLine;

internal record LeaveLineCommand(string LineCode) : IWashingCommand<ErrorOr<Success>>;