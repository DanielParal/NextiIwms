using System.Text.Json;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Users;
using Nexticz.Module.Mmo.Settings.Contracts.Users.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;

using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.MessagePublishers;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSosByCode;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Commands.CallSos;

internal class CallSosCommandHandler(
    ISender sender,
    ILogger<CallSosCommandHandler> logger,
    IClock clock,
    IWashingUnitOfWork unitOfWork,
    IMessagePublisher messagePublisher) : IRequestHandler<CallSosCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(CallSosCommand request, CancellationToken cancellationToken)
    {
        var existingWashingMachineSos = await sender.Send(new GetWashingMachineSosByCodeQuery(request.WashingMachineCode), cancellationToken);

        if (existingWashingMachineSos is null)
        {
            return await CreateAggregate(request, cancellationToken);
        }

        if (existingWashingMachineSos.IsHelpNeeded)
        {
            logger.LogInformation("Washing - washing machine already called for SOS. Washing machine code: {WashingMachineCode}.", 
                existingWashingMachineSos.Code);
            return Result.Success;
        }
        
        var sosCalledEvent = new WashingMachineSosCalledEvent(existingWashingMachineSos.Id, existingWashingMachineSos.Code, clock.UtcNowOffset);
        unitOfWork.AppendEvent(existingWashingMachineSos.Id, sosCalledEvent);
        
        logger.LogInformation("Washing - sos for washing machine: {WashingMachineCode} was called.", 
            request.WashingMachineCode);
        
        await PublishNotificationAsync(request.WashingMachineCode, cancellationToken);
        
        return Result.Success;
    }
    
    private async Task<ErrorOr<Success>> CreateAggregate(CallSosCommand request, CancellationToken cancellationToken)
    {
        var washingMachineFromSettings = await sender.Send(new GetWashingMachineResponsesQuery(), cancellationToken);
        if (washingMachineFromSettings.FirstOrDefault(x => x.Code.Equals(request.WashingMachineCode, StringComparison.InvariantCultureIgnoreCase)) is null)
        {
            logger.LogInformation("Washing - washing machine code does not exist in settings. Cannot call sos. Washing machine code: {WashingMachineCode}.", 
                request.WashingMachineCode);
            return WashingMachineSosErrors.ValidationWashingMachineCodeDoesNotExistInSettings;
        }
        
        var washingMachineSos = new WashingMachineSos(request.WashingMachineCode.ToUpperInvariant(), true);
        var washingMachineSosCreatedAndCalled = new WashingMachineSosCalledEvent(washingMachineSos.Id, washingMachineSos.Code, clock.UtcNowOffset);
        unitOfWork.StartStream<WashingMachineSosCalledEvent, WashingMachineSos>(washingMachineSos.Id, washingMachineSosCreatedAndCalled);
        logger.LogInformation("Washing - washing machine sos created and called. Washing machine code: {WashingMachineCode}.", 
            washingMachineSos?.Code);
        await PublishNotificationAsync(request.WashingMachineCode, cancellationToken);
        return Result.Success;
    }

    private async Task PublishNotificationAsync(string washingMachineCode, CancellationToken cancellationToken)
    {
        var usersForNotification = await sender.Send(new GetUsersByNotificationQuery(ReceivableNotificationContract.SosCalled), cancellationToken);
        if (usersForNotification.Length == 0)
        {
            logger.LogWarning("Washing - there are no users for notification {NotificationName}. WashingMachineCode: {WashingMachineCode}.", 
                nameof(ReceivableNotificationContract.SosCalled), washingMachineCode);
            return;
        }
        
        var sosSignalRMessage = new SosSignalRMessage(washingMachineCode);
        var signalRPublishedNotification = 
            new SignalRPublishedNotification(
                WashingMachineSosTranslations.SosCalledSignalRMessageTitle().TranslationValue,
                WashingMachineSosTranslations.SosCalledSignalRMessageDescription(washingMachineCode).TranslationValue,
                JsonSerializer.Serialize(sosSignalRMessage), 
                nameof(ReceivableNotificationContract.SosCalled),
                ModuleNameProvider.Name, 
                NotificationSeverityContract.Error,
                usersForNotification.Select(x => x.UserName).ToArray());
        
        await messagePublisher.PublishAsync(signalRPublishedNotification, cancellationToken);
    }
}