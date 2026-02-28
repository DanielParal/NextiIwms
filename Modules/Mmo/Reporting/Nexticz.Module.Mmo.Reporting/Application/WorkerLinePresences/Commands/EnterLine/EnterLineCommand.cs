using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Commands.EnterLine;

internal record EnterLineCommand(
    Guid ShiftId, string LineCode, Guid WorkerId, string WorkerName, DateTimeOffset EnteredAt) 
    : IReportingCommand<Success>;