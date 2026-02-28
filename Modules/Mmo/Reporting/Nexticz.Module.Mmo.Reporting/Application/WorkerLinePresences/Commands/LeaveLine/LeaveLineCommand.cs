using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Commands.LeaveLine;

internal record LeaveLineCommand(
    Guid ShiftId, string LineCode, Guid WorkerId, string WorkerName, DateTimeOffset LeftAt) 
    : IReportingCommand<Success>;