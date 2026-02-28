using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.CreateInactivity;

internal record CreateInactivityCommand(
    Guid ShiftId, string WashingMachineCode, string LineCode, 
    DateTimeOffset StartDate, DateTimeOffset EndDate, Guid? InactivityReasonId,
    InactivityType Type, bool IsPlanned, string? DeclaredBy) : IReportingCommand<ErrorOr<Success>>;