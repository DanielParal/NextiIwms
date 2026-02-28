using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.BreakSettings.GetBreakSettings;
using Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.CreateInactivity;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.CreateShift;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetLastShift;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;

internal class ShiftCreationOrchestrator(
    ISender sender,
    IClock clock,
    ShiftSettingManagerFactory shiftSettingManagerFactory) : IShiftCreationOrchestrator
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
    
    public async Task<Shift> EnsureCurrentShiftAsync(CancellationToken cancellationToken)
    {
        await SemaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            var now = clock.TenantNowOffset;
            var lastShift = await sender.Send(new GetLastShiftQuery(), cancellationToken);

            var shiftSettingManager = await shiftSettingManagerFactory.CreateAsync(cancellationToken);
            var lastShiftValue = lastShift.IsError ? null : lastShift.Value;
            var decision = ShiftCreationDecider.ShouldCreateNewShift(lastShiftValue, shiftSettingManager, now);
            if (!decision.ShouldCreate)
                return lastShift.Value;

            var createdShift = await sender.Send(new CreateShiftCommand(decision.NextShiftName!, decision.NextShiftSchedule!), cancellationToken);

            if (!createdShift.IsError)
            {
                await CreateBreaksAsync(createdShift.Value, now, cancellationToken);
            }

            return createdShift.Value;
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }
    
    private async Task CreateBreaksAsync(Shift shift, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var washingMachinesFromSettings = await sender.Send(new GetWashingMachineResponsesQuery(), cancellationToken);
        var breakSettings = await sender.Send(new GetBreakSettingsQuery(), cancellationToken);

        foreach (var breakSetting in breakSettings)
        {
            var breakTimeRange = breakSetting.GetBreakTimeRange(shift.Schedule.Start);
            if (!shift.IsWithinShift(breakTimeRange.Start))
                continue;
            
            if (breakTimeRange.Start < now)
                continue;

            await CreateBreaksForWashingMachinesAsync(shift, breakTimeRange, washingMachinesFromSettings, cancellationToken);
        }
    }
    
    private async Task CreateBreaksForWashingMachinesAsync(
        Shift shift,
        (DateTimeOffset Start, DateTimeOffset End) breakTimeRange,
        WashingMachineResponse[] washingMachines,
        CancellationToken cancellationToken)
    {
        foreach (var washingMachine in washingMachines)
        {
            foreach (var line in washingMachine.WashingMachineLines)
            {
                await sender.Send(new CreateInactivityCommand(
                        shift.Id, washingMachine.Code, line.Code, breakTimeRange.Start, breakTimeRange.End, null, InactivityType.Break, true, null), 
                    cancellationToken);
            }
        }
    }
}