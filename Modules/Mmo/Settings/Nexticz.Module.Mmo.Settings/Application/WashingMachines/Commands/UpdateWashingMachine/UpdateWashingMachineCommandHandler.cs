using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachineByCode;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.UpdateWashingMachine;

internal class UpdateWashingMachineCommandHandler (
    ILogger<UpdateWashingMachineCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender,
    ISettingsNotificationCollector notificationCollector)
    : IRequestHandler<UpdateWashingMachineCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateWashingMachineCommand request, CancellationToken cancellationToken)
    {
        var washingMachine = await sender.Send(new GetWashingMachineByCodeQuery(request.Code), cancellationToken);
        
        var validationResult = IsValid(washingMachine, request);
        
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var washingMachineLines = 
            request.UpdateWashingMachineRequest.WashingMachineLines
                .Select(x => new WashingMachineLine(x.Code, x.IsActive, 
                    new PrinterSettings(x.PrinterSettings?.Ip, x.PrinterSettings?.UserName, x.PrinterSettings?.Password, x.PrinterSettings?.CopyCount, (PrinterPageSize?)x.PrinterSettings?.PageSize)))
                .ToArray();
        
        var washingMachineSpeedsUpdatedEvent = new WashingMachineUpdatedEvent(
            washingMachine.Value.Id, request.UpdateWashingMachineRequest.Note, (WashingMachineStatus)request.UpdateWashingMachineRequest.Status, 
            request.UpdateWashingMachineRequest.Speed1, request.UpdateWashingMachineRequest.Speed2, request.UpdateWashingMachineRequest.Speed3,
            request.UpdateWashingMachineRequest.MaxWaterTemperature, request.UpdateWashingMachineRequest.MaxAirTemperature,
            request.UpdateWashingMachineRequest.MinWidth, washingMachineLines
        );
        
        unitOfWork.AppendEvent(washingMachine.Value.Id, washingMachineSpeedsUpdatedEvent);

        if (AreWashingMachineStatusOrLinesChanged(washingMachine.Value, request))
        {
            logger.LogInformation("Washing machine status or lines changed. Washing machine code: {WashingMachineCode}, new status: {Status}. Publishing notification.",
                washingMachine.Value.Code, request.UpdateWashingMachineRequest.Status);
            notificationCollector.AddNotification(
                new WashingMachineStatusUpdatedNotification(
                    washingMachine.Value.Code, 
                    request.UpdateWashingMachineRequest.Status, 
                    request.UpdateWashingMachineRequest.WashingMachineLines));
        }
        
        return Result.Updated;
    }

    private static bool AreWashingMachineStatusOrLinesChanged(WashingMachine washingMachine, UpdateWashingMachineCommand request)
    {
        if (washingMachine.Status != (WashingMachineStatus)request.UpdateWashingMachineRequest.Status)
            return true;
    
        var washingMachineLinesActiveState = washingMachine.WashingMachineLines
            .ToDictionary(line => line.Code, line => line.IsActive);
    
        foreach (var requestLine in request.UpdateWashingMachineRequest.WashingMachineLines)
        {
            if (!washingMachineLinesActiveState.TryGetValue(requestLine.Code, out var isActive) || isActive != requestLine.IsActive)
                return true;
        }
    
        return false;
    }
    
    private ErrorOr<Success> IsValid(ErrorOr<WashingMachine> washingMachine, UpdateWashingMachineCommand request)
    {
        if (!washingMachine.HasValue())
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(WashingMachine), request.Code);
            return washingMachine.Errors;
        }

        if (washingMachine.Value.NumberOfLines != request.UpdateWashingMachineRequest.WashingMachineLines.Length)
        {
            logger.LogInformation("Number of lines for {ObjectName} with code: {Code} do not match. We cannot update. Number of lines in property: {NumberOfLines} and number of lines in the list: {WashingMachineLinesLength}", 
                nameof(WashingMachine), request.Code, washingMachine.Value.NumberOfLines, request.UpdateWashingMachineRequest.WashingMachineLines.Length);

            return WashingMachineErrors.ValidationNumberOfLinesDoNotMatch(washingMachine.Value.NumberOfLines);
        }

        if ((WashingMachineStatus)request.UpdateWashingMachineRequest.Status == WashingMachineStatus.Working &&
            request.UpdateWashingMachineRequest.WashingMachineLines.All(x => !x.IsActive))
        {
            logger.LogInformation("All lines for {ObjectName} with code: {Code} are inactive and status is set to working. You need to change {ObjectName} status as well.", nameof(WashingMachine), request.Code, nameof(WashingMachine));
        
            return WashingMachineErrors.ValidationAllLinesAreInactiveAndStatusWorking;
        }
        
        var requestLineCodes = request.UpdateWashingMachineRequest.WashingMachineLines.Select(x => x.Code).ToHashSet();
        var existingLineCodes = washingMachine.Value.WashingMachineLines.Select(x => x.Code).ToHashSet();
        if (!requestLineCodes.SetEquals(existingLineCodes))
        {
            var existingLineCodesString = string.Join(", ", existingLineCodes);
            var requestLineCodesString = string.Join(", ", requestLineCodes);
            logger.LogInformation("Line codes mismatch for {ObjectName} with code: {Code}. Existing codes: {ExistingCodes}, Requested codes: {RequestedCodes}", 
                nameof(WashingMachine), request.Code, existingLineCodesString, requestLineCodesString);
        
            return WashingMachineErrors.ValidationLineCodesMismatch(existingLineCodesString, requestLineCodesString);
        }
        
        return Result.Success;
    }
}