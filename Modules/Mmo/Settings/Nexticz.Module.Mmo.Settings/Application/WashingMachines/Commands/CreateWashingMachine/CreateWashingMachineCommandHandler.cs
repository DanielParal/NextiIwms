using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagings;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachineByCode;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate.Events;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity.Events;


namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.CreateWashingMachine;

internal class CreateWashingMachineCommandHandler (
    ILogger<CreateWashingMachineCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender,
    ISettingsNotificationCollector notificationCollector)
    : IRequestHandler<CreateWashingMachineCommand, ErrorOr<WashingMachine>>
{
    public async Task<ErrorOr<WashingMachine>> Handle(CreateWashingMachineCommand request, CancellationToken cancellationToken)
    {
        var existingWashingMachine = await sender.Send(new GetWashingMachineByCodeQuery(request.CreateWashingMachineRequest.Code), cancellationToken);

        if (existingWashingMachine.HasValue())
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} already exists. Nothing to create.", nameof(WashingMachine), request.CreateWashingMachineRequest.Code);
            return WashingMachineErrors.ValidationCodeAlreadyExists(request.CreateWashingMachineRequest.Code);
        }
        
        var packagings = await sender.Send(new GetPackagingsQuery(new BaseFilteringParams()), cancellationToken);
        var notSetWashingMachineSpeed = new WashingMachineSpeed(request.CreateWashingMachineRequest.Code, WashingMachineSpeedLevel.NotSet);
        
        foreach (var packaging in packagings.Data)
        {
            var speed = packaging.WashingMachineSpeeds.FirstOrDefault(x => x.WashingMachineCode == request.CreateWashingMachineRequest.Code);

            if (speed != null)
            {
                logger.LogWarning("Packaging with code: {Code} already has washing machine speed with code: {WashingMachineCode}", packaging.Code, request.CreateWashingMachineRequest.Code);
                continue;
            }
            
            var washingMachineSpeedCreatedEvent = new PackagingWashingMachineSpeedCreatedEvent(packaging.Id, packaging.Code, notSetWashingMachineSpeed);
            unitOfWork.AppendEvent(packaging.Id, washingMachineSpeedCreatedEvent);
        }
        
        var washingMachineLines = CreateWashingMachineLines(request.CreateWashingMachineRequest.NumberOfLines, request.CreateWashingMachineRequest.Code);
        
        var washingMachine = new WashingMachine(
            request.CreateWashingMachineRequest.Code, request.CreateWashingMachineRequest.Note, (WashingMachineStatus)request.CreateWashingMachineRequest.Status, 
            request.CreateWashingMachineRequest.Length, request.CreateWashingMachineRequest.MinWidth,
            request.CreateWashingMachineRequest.MaxWidth, request.CreateWashingMachineRequest.MaxHeight, request.CreateWashingMachineRequest.MaxWaterTemperature,
            request.CreateWashingMachineRequest.MaxAirTemperature, request.CreateWashingMachineRequest.NumberOfLines, request.CreateWashingMachineRequest.Speed1,
            request.CreateWashingMachineRequest.Speed2, request.CreateWashingMachineRequest.Speed3, washingMachineLines);
        
        var washingMachineCreatedEvent = new WashingMachineCreatedEvent(
            washingMachine.Id,  
            request.CreateWashingMachineRequest.Code, request.CreateWashingMachineRequest.Note, (WashingMachineStatus)request.CreateWashingMachineRequest.Status, 
            request.CreateWashingMachineRequest.Length, request.CreateWashingMachineRequest.MinWidth,
            request.CreateWashingMachineRequest.MaxWidth, request.CreateWashingMachineRequest.MaxHeight, request.CreateWashingMachineRequest.MaxWaterTemperature,
            request.CreateWashingMachineRequest.MaxAirTemperature, request.CreateWashingMachineRequest.NumberOfLines, request.CreateWashingMachineRequest.Speed1,
            request.CreateWashingMachineRequest.Speed2, request.CreateWashingMachineRequest.Speed3, washingMachineLines);
        
        unitOfWork.StartStream<WashingMachineCreatedEvent, WashingMachine>(washingMachine.Id, washingMachineCreatedEvent);
        
        var notification = new WashingMachineCreatedNotification(
            washingMachine.Code, (WashingMachineStatusContract)washingMachine.Status,
            washingMachineLines.Select(x => 
                new WashingMachineLineContract(x.Code, x.IsActive, new PrinterSettingsContract(null, null, null, null, null)))
                .ToArray());
        notificationCollector.AddNotification(notification);
        
        return washingMachine;
    }

    private static WashingMachineLine[] CreateWashingMachineLines(int numberOfLines, string washingMachineCode)
    {
        var washingMachineLines = new WashingMachineLine[numberOfLines];
        for (var i = 0; i < numberOfLines; i++)
        {
            var code = $"{washingMachineCode}_L{1+i}";
            washingMachineLines[i] = new WashingMachineLine(code, true, new PrinterSettings(null, null, null, null, null));
        }
        
        return washingMachineLines;
    }
}