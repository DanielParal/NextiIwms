using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings.Queries.GetShiftSettings;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetLastShift;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetNextToLastShift;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.CreateShift;

internal class CreateShiftCommandHandler(
    ISender sender,
    ILogger<CreateShiftCommandHandler> logger,
    IClock clock,
    IReportingUnitOfWork unitOfWork) : IRequestHandler<CreateShiftCommand, ErrorOr<Shift>>
{
    public async Task<ErrorOr<Shift>> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
    {
       await MovePreviousShiftsBackAsync(cancellationToken);
       return await CreateShiftAsync(request.NextShiftName, request.NextShiftSchedule, cancellationToken);
    }

    private async Task MovePreviousShiftsBackAsync(CancellationToken cancellationToken)
    {
        var lastShift = await sender.Send(new GetLastShiftQuery(), cancellationToken);

        if (!lastShift.IsError)
        {
            var lastShiftMovedToNextToLastEvent = new LastShiftMovedToNextToLastEvent(lastShift.Value.Id);
            unitOfWork.AppendEvent(lastShift.Value.Id, lastShiftMovedToNextToLastEvent);
            logger.LogInformation("Reporting - Last shift moved to next to last. Shift id: {ShiftId}", lastShift.Value.Id);
        }

        var nextToLastShift = await sender.Send(new GetNextToLastShiftQuery(), cancellationToken);
        
        if (!nextToLastShift.IsError)
        {
            var nextToLastShiftMovedBackEvent = new NextToLastShiftMovedBackEvent(nextToLastShift.Value.Id);
            unitOfWork.AppendEvent(nextToLastShift.Value.Id, nextToLastShiftMovedBackEvent);
            logger.LogInformation("Reporting - Next to last shift moved back: {ShiftId}", nextToLastShift.Value.Id);
        }
    }

    private async Task<Shift> CreateShiftAsync(string nextShiftName, ShiftSchedule nextShiftSchedule, CancellationToken cancellationToken)
    {
        var washingMachines = await GetWashingMachinesFromSettingsAsync(cancellationToken);
       
        var shift = new Shift(nextShiftName, true, false, nextShiftSchedule, clock.UtcNowOffset, washingMachines);
        var shiftCreatedEvent = new ShiftCreatedEvent(shift.Id, shift.Name, shift.Schedule, shift.CreatedAt, shift.WashingMachines);
        unitOfWork.StartStream<ShiftCreatedEvent, Shift>(shift.Id, shiftCreatedEvent);

        logger.LogInformation("Reporting - New shift created. Shift id: {ShiftId}, Shift name: {ShiftName}, " +
                              "Shift start date: {ShiftStartDate}, Shift end date: {ShiftEndDate}", 
            shift.Id, shift.Name, shift.Schedule.Start, shift.Schedule.End);
        
        return shift;
    }

    private async Task<WashingMachine[]> GetWashingMachinesFromSettingsAsync(CancellationToken cancellationToken)
    {
        var washingMachineFromSettings = 
            await sender.Send(new GetWashingMachineResponsesQuery(), cancellationToken);
        
        return washingMachineFromSettings
            .Select(x => new WashingMachine(x.Code,
                x.WashingMachineLines
                    .Select(y => 
                        new WashingMachineLine(y.Code))
                    .ToArray()))
            .ToArray();
    }
}